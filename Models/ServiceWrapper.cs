using Gateway.Dtos.Request;
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
  public readonly CircuitBreaker CircuitBreaker;
  public readonly ServiceInstanceDto InstanceDto;

  public ServiceWrapper(ServiceInstanceDto instance) {
    Channel = GrpcChannel.ForAddress(instance.Url());
    Client = (T)Activator.CreateInstance(typeof(T), Channel)!;
    CircuitBreaker = new CircuitBreaker(instance);
    InstanceDto = instance;
  }

  public async ValueTask DisposeAsync() {
    await Channel.ShutdownAsync();
    Channel.Dispose();
    GC.SuppressFinalize(this);
  }
}
