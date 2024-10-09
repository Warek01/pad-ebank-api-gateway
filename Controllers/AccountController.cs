using Asp.Versioning;
using Gateway.Exceptions;
using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("/Api/v{v:apiVersion}/[controller]")]
public class AccountController(
   AccountServiceLoadBalancer loadBalancer,
   ILogger<AccountController> logger
) : ControllerBase {
   [HttpPost("Register")]
   public async Task<ActionResult> Register(RegisterCredentials credentials) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"Load balancer gave {service.InstanceDto}");

         AuthResult result = await service.CircuitBreaker.Execute<AuthResult>(
            async (cts) => await service.Client.RegisterAsync(credentials, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ThrowAccountServiceError(result.Error);
         }

         return Ok(result.Credentials);
      }
      catch (CircuitOpenException) {
         return await Register(credentials);
      }
   }

   [HttpPost("Login")]
   public async Task<ActionResult> Login(LoginCredentials credentials) {
      var service = await loadBalancer.GetServiceAsync();
      AuthResult result = await service.Client.LoginAsync(credentials);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok(result.Credentials);
   }

   [HttpPost("Profile")]
   public async Task<ActionResult> GetProfile(GetProfileOptions options) {
      var service = await loadBalancer.GetServiceAsync();
      GetProfileResult result = await service.Client.GetProfileAsync(options);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok(result.Profile);
   }

   [HttpPost("Currency/Add")]
   public async Task<ActionResult> AddCurrency(AddCurrencyOptions options) {
      var service = await loadBalancer.GetServiceAsync();
      AddCurrencyResult? result = await service.Client.AddCurrencyAsync(options);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok();
   }

   [HttpPost("Currency/Change")]
   public async Task<ActionResult> ChangeCurrency(ChangeCurrencyOptions options) {
      var service = await loadBalancer.GetServiceAsync();
      ChangeCurrencyResult? result = await service.Client.ChangeCurrencyAsync(options);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok();
   }

   [HttpPost("Transaction/Validate")]
   public async Task<ActionResult> CanPerformTransaction(TransactionData data) {
      var service = await loadBalancer.GetServiceAsync();
      CanPerformTransactionResult? result = await service.Client.CanPerformTransactionAsync(data);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok();
   }

   [HttpPost("Card/Block")]
   public async Task<ActionResult> BlockCard(CardIdentifier cardIdentifier) {
      var service = await loadBalancer.GetServiceAsync();
      BlockCardResult? result = await service.Client.BlockCardAsync(cardIdentifier);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok();
   }

   [HttpPost("Card/Unblock")]
   public async Task<ActionResult> UnblockCard(CardIdentifier cardIdentifier) {
      var service = await loadBalancer.GetServiceAsync();
      UnblockCardResult? result = await service.Client.UnblockCardAsync(cardIdentifier);

      if (result.Error is not null) {
         return ThrowAccountServiceError(result.Error);
      }

      return Ok();
   }

   private ActionResult ThrowAccountServiceError(ServiceError error) {
      ServiceErrorCode code = error.Code;
      string message = error.Message;

      return code switch {
         ServiceErrorCode.Conflict => Conflict(message),
         ServiceErrorCode.BadRequest => BadRequest(message),
         ServiceErrorCode.NotFound => NotFound(message),
         ServiceErrorCode.Unauthorized => Unauthorized(message),
         _ => StatusCode(500, error),
      };
   }
}
