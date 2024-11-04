using Gateway.Exceptions;
using Gateway.Services;
using Grpc.Core;
using Grpc.Net.Client;

namespace Gateway.Models;

/// <summary>
/// A wrapper that holds the service, connection channel and circuit breaker
/// </summary>
/// <typeparam name="T">gRPC client type</typeparam>
public class ServiceWrapper<T> : IAsyncDisposable where T : ClientBase {
   public readonly T Client;
   public readonly GrpcChannel Channel;
   public readonly CircuitBreaker<T> CircuitBreaker;
   public readonly RegistryEntry SdInstance;

   public ServiceWrapper(RegistryEntry sdInstance, ServiceDiscoveryService serviceDiscoveryService) {
      Console.WriteLine(sdInstance);
      Channel = GrpcChannel.ForAddress(sdInstance.GrpcUri!);
      Client = (T)Activator.CreateInstance(typeof(T), Channel)!;
      CircuitBreaker = new CircuitBreaker<T>(this, serviceDiscoveryService);
      SdInstance = sdInstance;
   }

   public static async Task<ServiceWrapper<T>> GetService(ServiceDiscoveryService serviceDiscoveryService, string serviceName) {
      RegistryEntry? dto = await serviceDiscoveryService.GetInstance(serviceName);

      if (dto is null) {
         throw new ServiceUnavailableException($"{serviceName} is unavailable");
      }

      return new ServiceWrapper<T>(dto, serviceDiscoveryService);
   }

   public async ValueTask DisposeAsync() {
      await Channel.ShutdownAsync();
      Channel.Dispose();
      GC.SuppressFinalize(this);
   }
}
