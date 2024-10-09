using Gateway.Dtos.Request;
using Gateway.Exceptions;
using Gateway.Services;
using Grpc.Core;

namespace Gateway.Models;

public abstract class LoadBalancer<T>(
   ServiceDiscoveryService serviceDiscovery,
   string serviceName
) where T : ClientBase {
   private readonly List<ServiceWrapper<T>> _servicePool = [];
   private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
   private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(30);

   private bool _isInstantiated = false;
   private int _roundRobinIndex = 0;

   public async Task RemoveServiceAsync(ServiceWrapper<T> service) {
      await serviceDiscovery.RemoveInstance(service.InstanceDto.Host);
      _servicePool.Remove(service);
      _roundRobinIndex = Math.Max(_roundRobinIndex, _servicePool.Count - 1);
      await service.DisposeAsync();
   }

   public async Task<ServiceWrapper<T>> GetServiceAsync() {
      if (!_isInstantiated) {
         await Init();
      }

      return Next();
   }

   private async Task ScheduleUpdates() {
      while (true) {
         await Task.Delay(_updateInterval);
         await UpdateLoadBalancer();
      }
   }

   private async Task UpdateLoadBalancer() {
      List<ServiceInstanceDto> dtos = await serviceDiscovery.GetInstanceUrls(serviceName);
      _roundRobinIndex = 0;

      await _semaphore.WaitAsync();

      try {
         if (_isInstantiated) {
            UpdateServiceInstances(dtos);
         }
         else {
            CreateServiceInstances(dtos);
         }
      }
      finally {
         _semaphore.Release();
      }
   }

   private void CreateServiceInstances(List<ServiceInstanceDto> dtos) {
      foreach (ServiceInstanceDto dto in dtos) {
         var service = new ServiceWrapper<T>(dto, this);
         _servicePool.Add(service);
      }
   }

   private void UpdateServiceInstances(List<ServiceInstanceDto> dtos) {
      List<ServiceWrapper<T>> servicesToRemove = [.._servicePool];

      foreach (ServiceInstanceDto dto in dtos) {
         ServiceWrapper<T>? service = _servicePool.Find(s => s.InstanceDto.Host == dto.Host);

         if (service is null) {
            service = new ServiceWrapper<T>(dto, this);
            _servicePool.Add(service);
         }
         else {
            servicesToRemove.Remove(service);
         }
      }

      // Remove and dispose of services no longer needed
      foreach (ServiceWrapper<T> s in servicesToRemove) {
         _servicePool.Remove(s);
         _ = s.DisposeAsync();
      }
   }

   private ServiceWrapper<T> Next() {
      _semaphore.Wait();

      try {
         ServiceWrapper<T> service;

         switch (_servicePool.Count) {
            case 0:
               throw new ServiceUnavailableException(serviceName);
            case 1: {
               service = _servicePool[0];

               if (!service.CircuitBreaker.CanPerform()) {
                  throw new ServiceUnavailableException(serviceName);
               }

               return service;
            }
            default: {
               int unhealthyServicesCount = 0;
               bool canPerform = false;

               while (!canPerform && unhealthyServicesCount < _servicePool.Count) {
                  service = _servicePool[_roundRobinIndex];
                  _roundRobinIndex++;

                  if (_roundRobinIndex >= _servicePool.Count) {
                     _roundRobinIndex = 0;
                  }

                  canPerform = service.CircuitBreaker.CanPerform();

                  if (canPerform) {
                     return service;
                  }

                  unhealthyServicesCount++;
               }

               throw new ServiceUnavailableException(serviceName);
            }
         }
      }
      finally {
         _semaphore.Release();
      }
   }

   private async Task Init() {
      if (_isInstantiated) {
         return;
      }

      await UpdateLoadBalancer();
      _ = ScheduleUpdates();
      _isInstantiated = true;
   }
}
