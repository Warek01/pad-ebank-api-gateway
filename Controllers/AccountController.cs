using Asp.Versioning;
using Gateway.Exceptions;
using Gateway.Helpers;
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

         logger.LogInformation($"[{nameof(Register)}] Load balancer gave {service.InstanceDto}");

         AuthResult result = await service.CircuitBreaker.Execute<AuthResult>(
            async (cts) => await service.Client.RegisterAsync(credentials, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result.Credentials);
      }
      catch (CircuitOpenException) {
         return await Register(credentials);
      }
   }

   [HttpPost("Login")]
   public async Task<ActionResult> Login(LoginCredentials credentials) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(Login)}] Load balancer gave {service.InstanceDto}");

         AuthResult result = await service.CircuitBreaker.Execute<AuthResult>(
            async (cts) => await service.Client.LoginAsync(credentials, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result.Credentials);
      }
      catch (CircuitOpenException) {
         return await Login(credentials);
      }
   }

   [HttpPost("Profile")]
   public async Task<ActionResult> GetProfile(GetProfileOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(GetProfile)}] Load balancer gave {service.InstanceDto}");

         GetProfileResult result = await service.CircuitBreaker.Execute<GetProfileResult>(
            async (cts) => await service.Client.GetProfileAsync(options, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result.Profile);
      }
      catch (CircuitOpenException) {
         return await GetProfile(options);
      }
   }

   [HttpPost("Currency/Add")]
   public async Task<ActionResult> AddCurrency(AddCurrencyOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(AddCurrency)}] Load balancer gave {service.InstanceDto}");

         AddCurrencyResult? result = await service.CircuitBreaker.Execute<AddCurrencyResult>(
            async (cts) => await service.Client.AddCurrencyAsync(options, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok();
      }
      catch (CircuitOpenException) {
         return await AddCurrency(options);
      }
   }

   [HttpPost("Currency/Change")]
   public async Task<ActionResult> ChangeCurrency(ChangeCurrencyOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(ChangeCurrency)}] Load balancer gave {service.InstanceDto}");

         ChangeCurrencyResult? result = await service.CircuitBreaker.Execute<ChangeCurrencyResult>(
            async (cts) => await service.Client.ChangeCurrencyAsync(options, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok();
      }
      catch (CircuitOpenException) {
         return await ChangeCurrency(options);
      }
   }

   [HttpPost("Transaction/Validate")]
   public async Task<ActionResult> CanPerformTransaction(TransactionData data) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(CanPerformTransaction)}] Load balancer gave {service.InstanceDto}");

         CanPerformTransactionResult? result = await service.CircuitBreaker.Execute<CanPerformTransactionResult>(
            async (cts) => await service.Client.CanPerformTransactionAsync(data, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok();
      }
      catch (CircuitOpenException) {
         return await CanPerformTransaction(data);
      }
   }

   [HttpPost("Card/Block")]
   public async Task<ActionResult> BlockCard(CardIdentifier cardIdentifier) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(BlockCard)}] Load balancer gave {service.InstanceDto}");

         BlockCardResult? result = await service.CircuitBreaker.Execute<BlockCardResult>(
            async (cts) => await service.Client.BlockCardAsync(cardIdentifier, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok();
      }
      catch (CircuitOpenException) {
         return await BlockCard(cardIdentifier);
      }
   }

   [HttpPost("Card/Unblock")]
   public async Task<ActionResult> UnblockCard(CardIdentifier cardIdentifier) {
      try {
         ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(UnblockCard)}] Load balancer gave {service.InstanceDto}");

         UnblockCardResult? result = await service.CircuitBreaker.Execute<UnblockCardResult>(
            async (cts) => await service.Client.UnblockCardAsync(cardIdentifier, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok();
      }
      catch (CircuitOpenException) {
         return await UnblockCard(cardIdentifier);
      }
   }
   
   [HttpGet("Lobby")]
   public async Task<ActionResult<string>> AccountLobby() {
      ServiceWrapper<AccountServiceClient> service = await loadBalancer.GetServiceAsync();
      return $"ws://{service.InstanceDto.Host}:3200/account";
   }
}
