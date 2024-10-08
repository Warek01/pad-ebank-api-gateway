using Gateway.Dtos.Request;
using Gateway.Exceptions;
using Serilog;

namespace Gateway.Models;

public class CircuitBreaker(ServiceInstanceDto instance) {
  private enum CircuitState {
    Closed,
    Open,
    HalfOpen,
  }

  private const int FailureThreshold = 3;
  private readonly TimeSpan _retryDelay = TimeSpan.FromSeconds(1);

  private readonly TimeSpan _closedTimeout = TimeSpan.FromSeconds(10);
  // private readonly TimeSpan _halfOpenTimeout = TimeSpan.FromSeconds(10);
  // private readonly TimeSpan _halfOpenAllowRequestInterval = TimeSpan.FromSeconds(1);

  private int _failureCount = 0;

  private DateTime _lastFailureTime = DateTime.MinValue;

  // private DateTime _lastSuccessTime = DateTime.MinValue;
  private CircuitState _state = CircuitState.Closed;

  /// <returns>Return value of action</returns>
  /// <exception cref="CircuitOpenException"></exception>
  public async Task<T> Execute<T>(Func<Task<T>> action) {
    try {
      T result = await action();
      ExecuteSuccess();
      return result;
    }
    catch (Exception ex) {
      ExecuteFailure();
      Log.Logger.Error(ex.ToString());

      if (!CanPerform()) {
        RemoveInstance();
        throw new CircuitOpenException(string.Empty);
      }

      await Task.Delay(_retryDelay);
      return await Execute<T>(action);
    }
  }

  private void ExecuteSuccess() {
    _failureCount = 0;
    // _lastSuccessTime = DateTime.Now;
    _state = CircuitState.Closed;
  }

  private void ExecuteFailure() {
    Log.Error($"Execute failure for {instance}, {FailureThreshold - _failureCount} retries remaining");
    _failureCount++;
    _lastFailureTime = DateTime.Now;

    if (_failureCount >= FailureThreshold) {
      Log.Error($"Circuit opened for {instance}");
      _state = CircuitState.Open;
    }
  }

  private void RemoveInstance() {
    // TODO: implement
  }

  public bool CanPerform() {
    DateTime now = DateTime.Now;

    // TODO: implement half-opened logic

    // don't allow if too many requests are made during half-open
    // if (_state == CircuitState.HalfOpen && now - _lastSuccessTime < _halfOpenAllowRequestInterval) {
    //   return false;
    // }

    // half-open after _closedTimeout if opened
    if (_state == CircuitState.Open && now - _lastFailureTime >= _closedTimeout) {
      _state = CircuitState.HalfOpen;
      Log.Logger.Information($"Circuit breaker is half-opened for {instance}");
    }

    // close after _closedTimeout if half-opened
    // if (_state == CircuitState.HalfOpen && now - _lastFailureTime >= _halfOpenTimeout) {
    //   ChangeState(CircuitState.Closed);
    // }

    return _state != CircuitState.Open;
  }
}
