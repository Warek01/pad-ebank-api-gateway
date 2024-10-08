using Gateway.Dtos.Request;
using Gateway.Exceptions;
using Gateway.Services;
using Grpc.Core;

namespace Gateway.Models;

public abstract class LoadBalancer<T>(
  ServiceDiscoveryService serviceDiscovery,
  string serviceName
) where T : ClientBase {
  private bool _instancesLoaded = false;
  private readonly List<ServiceWrapper<T>> _servicePool = [];
  private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
  private int _index = 0;

  public async Task<ServiceWrapper<T>> GetServiceAsync() {
    if (!_instancesLoaded) {
      await Init();
    }

    return Next();
  }

  private async Task ScheduleUpdate() {
    TimeSpan interval = TimeSpan.FromSeconds(30);

    while (true) {
      await Task.Delay(interval);
      await UpdateInstances();
    }
  }

  private async Task UpdateInstances() {
    Task<List<ServiceInstanceDto>> instanceTask = serviceDiscovery.GetInstanceUrls(serviceName);

    await Task.WhenAll(
      instanceTask,
      _semaphore.WaitAsync()
    );

    List<ServiceInstanceDto> dtos = instanceTask.Result;
    _index = 0;

    try {
      IEnumerable<Task> disposeTasks = _servicePool.Select(service => service.DisposeAsync().AsTask());
      await Task.WhenAll(disposeTasks);
      _servicePool.Clear();

      foreach (ServiceInstanceDto dto in dtos) {
        var service = new ServiceWrapper<T>(dto);
        _servicePool.Add(service);
      }
    }
    finally {
      _semaphore.Release();
    }
  }

  private ServiceWrapper<T> Next() {
    _semaphore.Wait();

    try {
      ServiceWrapper<T> service;
      int iteratedServicesCount = 0;

      do {
        service = _servicePool[_index];
        _index++;
        iteratedServicesCount++;

        if (_index >= _servicePool.Count) {
          _index = 0;
        }

        // if all services are unavailable
        if (iteratedServicesCount == _servicePool.Count) {
          throw new ServiceUnavailableException(serviceName);
        }
      } while (!service.CircuitBreaker.CanPerform());

      return service;
    }
    finally {
      _semaphore.Release();
    }
  }

  private async Task Init() {
    if (_instancesLoaded) {
      return;
    }

    await UpdateInstances();
    _ = ScheduleUpdate();
    _instancesLoaded = true;
  }
}
