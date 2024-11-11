using Asp.Versioning;
using Gateway.Exceptions;
using Gateway.Helpers;
using Gateway.Models;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("/Api/v{v:apiVersion}/[controller]")]
[SwaggerResponse(StatusCodes.Status429TooManyRequests)]
[SwaggerResponse(StatusCodes.Status500InternalServerError)]
[SwaggerTag("Account resource (bound to Account microservice)")]
public class AccountController(
   ServiceDiscoveryService serviceDiscoveryService,
   ILogger<AccountController> logger
) : ControllerBase {
   [SwaggerOperation("Register a user", "Register a user with the register credentials")]
   [SwaggerResponse(StatusCodes.Status201Created, "Register successful", typeof(AuthCredentials))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status409Conflict, "User already exists")]
   [HttpPost("Register")]
   public async Task<ActionResult> Register(RegisterCredentials credentials) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(Register)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerResponse(StatusCodes.Status200OK, "Login successful", typeof(AuthCredentials))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status401Unauthorized, "Wrong password")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Login")]
   public async Task<ActionResult> Login(LoginCredentials credentials) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(Login)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Get profile of a user")]
   [SwaggerResponse(StatusCodes.Status200OK, "Profile response", typeof(Profile))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Profile")]
   public async Task<ActionResult> GetProfile(GetProfileOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(GetProfile)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Add money to card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Operation successful")]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Currency/Add")]
   public async Task<ActionResult> AddCurrency(AddCurrencyOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(AddCurrency)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Change the currency type on a card (EUR, USD, MDL)")]
   [SwaggerResponse(StatusCodes.Status200OK, "Operation successful")]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Currency/Change")]
   public async Task<ActionResult> ChangeCurrency(ChangeCurrencyOptions options) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(ChangeCurrency)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Check if the given card can perform a transaction (has enough money)")]
   [SwaggerResponse(StatusCodes.Status200OK, "Operation successful")]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Transaction/Validate")]
   public async Task<ActionResult> CanPerformTransaction(TransactionData data) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(CanPerformTransaction)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Block the card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Operation successful")]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Card/Block")]
   public async Task<ActionResult> BlockCard(CardIdentifier cardIdentifier) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(BlockCard)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Unblock the card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Operation successful")]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Schema validation error")]
   [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
   [HttpPost("Card/Unblock")]
   public async Task<ActionResult> UnblockCard(CardIdentifier cardIdentifier) {
      try {
         ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(UnblockCard)}] Load balancer gave {service.SdInstance}");

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

   [SwaggerOperation("Get a lobby WebSocket endpoint")]
   [SwaggerResponse(StatusCodes.Status200OK, "The WebSocket endpoint", typeof(string))]
   [HttpGet("Lobby")]
   public async Task<ActionResult<string>> AccountLobby() {
      ServiceWrapper<AccountServiceClient> service = await GetServiceAsync();
      return $"ws://{service.SdInstance.HttpUri!.Host}:3200/account";
   }

   private Task<ServiceWrapper<AccountServiceClient>> GetServiceAsync() {
      return ServiceWrapper<AccountServiceClient>.GetServiceAsync(serviceDiscoveryService, ServiceNames.AccountService);
   }
}
