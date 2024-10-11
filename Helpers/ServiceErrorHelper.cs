using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Helpers;

public static class ServiceErrorHelper {
   public static ActionResult ServiceErrorToActionResult(ServiceError error) {
      ServiceErrorCode code = error.Code;
      string message = error.Message;

      return code switch {
         ServiceErrorCode.Conflict => new ConflictObjectResult(message),
         ServiceErrorCode.BadRequest => new BadRequestObjectResult(message),
         ServiceErrorCode.NotFound => new NotFoundObjectResult(message),
         ServiceErrorCode.Unauthorized => new UnauthorizedObjectResult(message),
         _ => new ObjectResult(new {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = "An unexpected error occurred.",
            ErrorCode = "INTERNAL_SERVER_ERROR",
            Timestamp = DateTime.UtcNow
         }),
      };
   }
}
