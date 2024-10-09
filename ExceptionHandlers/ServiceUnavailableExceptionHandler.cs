using Gateway.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.ExceptionHandlers;

public class ServiceUnavailableExceptionHandler(ILogger<ServiceUnavailableExceptionHandler> logger)
   : IExceptionHandler {
   public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken
   ) {
      if (exception is not ServiceUnavailableException) {
         return false;
      }

      logger.LogError(exception, "ServiceUnavailableException occurred: {Message}", exception.Message);

      var problemDetails = new ProblemDetails {
         Status = StatusCodes.Status503ServiceUnavailable,
         Title = "Service unavailable",
      };

      httpContext.Response.Headers.Append("Retry-After", "30s");
      await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

      return true;
   }
}
