using Gateway.Exceptions;
using Gateway.Services;
using Grpc.Core;
using Serilog;

namespace Gateway.Models;

public class CircuitBreaker<TService>(
   ServiceWrapper<TService> service,
   ServiceDiscoveryService serviceDiscoveryService
) where TService : ClientBase {
   private enum CircuitState {
      Closed,
      Open,
      HalfOpen,
   }

   private readonly bool _removeOnOpen =
      bool.Parse(Environment.GetEnvironmentVariable("CIRCUIT_BREAKER_REMOVE_ON_OPEN")!);

   private const int MaxFailuresAllowed = 3;
   private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(3);
   private readonly TimeSpan _timeoutDelay = TimeSpan.FromSeconds(5);
   private readonly TimeSpan _openTimeout = TimeSpan.FromSeconds(30);

   private int _failureCount = 0;
   private DateTime _lastFailureTime = DateTime.MinValue;
   private CircuitState _state = CircuitState.Closed;

   public async Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> action) {
      var actionCts = new CancellationTokenSource();
      var timeoutCts = new CancellationTokenSource();

      try {
         Task<TResult> resultTask = action(actionCts.Token);
         Task timeoutTask = Task.Delay(_timeoutDelay, timeoutCts.Token);
         Task completedFirst = await Task.WhenAny(resultTask, timeoutTask);

         if (completedFirst == timeoutTask) {
            await actionCts.CancelAsync();
            throw new TimeoutException();
         }

         await timeoutCts.CancelAsync();
         TResult res = await resultTask;
         ExecuteSuccess();

         return res;
      }
      catch (Exception ex) {
         ExecuteFailure();
         Log.Logger.Error(ex.Message);

         if (!CanPerform()) {
            if (_removeOnOpen) {
               await RemoveInstance();
            }

            throw new CircuitOpenException(string.Empty);
         }

         await Task.Delay(_retryDelay);

         return await Execute<TResult>(action);
      }
      finally {
         await actionCts.CancelAsync();
         await timeoutCts.CancelAsync();
         actionCts.Dispose();
         timeoutCts.Dispose();
      }
   }

   private void ExecuteSuccess() {
      _failureCount = 0;
      _state = CircuitState.Closed;
   }

   private void ExecuteFailure() {
      Log.Error($"Execute failure for {service.InstanceDto}, {MaxFailuresAllowed - _failureCount} retries remaining");
      _failureCount++;
      _lastFailureTime = DateTime.Now;

      if (_failureCount >= MaxFailuresAllowed) {
         Log.Error($"Circuit opened for {service.InstanceDto}");
         _state = CircuitState.Open;
      }
   }

   private async Task RemoveInstance() {
      await serviceDiscoveryService.RemoveInstance(service.InstanceDto.Host);
      Log.Logger.Information($"Removed service instance {service.InstanceDto}");
   }

   public bool CanPerform() {
      DateTime now = DateTime.Now;

      // half-open after _closedTimeout if opened
      if (_state == CircuitState.Open && now - _lastFailureTime >= _openTimeout) {
         _state = CircuitState.HalfOpen;
         Log.Logger.Information($"Circuit breaker is half-opened for {service.InstanceDto}");
         _failureCount = MaxFailuresAllowed - 1;
      }

      return _state != CircuitState.Open;
   }
}
