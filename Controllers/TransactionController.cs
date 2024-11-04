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
public class TransactionController(
   ServiceDiscoveryService serviceDiscoveryService,
   ILogger<TransactionController> logger
) : ControllerBase {
   [SwaggerOperation(Summary = "Transfer money from one account to another")]
   [SwaggerResponse(StatusCodes.Status200OK, "Transfer completed successfully.", typeof(TransferResult))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid transfer data.")]
   [HttpPost("Currency/Transfer")]
   public async Task<ActionResult<TransferResult>> TransferCurrency(TransferData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(TransferCurrency)}] Load balancer gave {service.SdInstance}");

         TransferResult result = await service.CircuitBreaker.Execute<TransferResult>(
            async (cts) => await service.Client.TransferCurrencyAsync(data, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result);
      }
      catch (CircuitOpenException) {
         return await TransferCurrency(data);
      }
   }

   [SwaggerOperation(Summary = "Deposit money into a card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Deposit completed successfully.", typeof(DepositResult))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid deposit data.")]
   [HttpPost("Currency/Deposit")]
   public async Task<ActionResult<DepositResult>> DepositCurrency(DepositData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(DepositCurrency)}] Load balancer gave {service.SdInstance}");

         DepositResult result = await service.CircuitBreaker.Execute<DepositResult>(
            async (cts) => await service.Client.DepositCurrencyAsync(data, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result);
      }

      catch (CircuitOpenException) {
         return await DepositCurrency(data);
      }
   }

   [SwaggerOperation(Summary = "Withdraw money from a card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Withdrawal completed successfully.", typeof(WithdrawResult))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid withdrawal data.")]
   [HttpPost("Currency/Withdraw")]
   public async Task<ActionResult<WithdrawResult>> WithdrawCurrency(WithdrawData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(WithdrawCurrency)}] Load balancer gave {service.SdInstance}");

         WithdrawResult result = await service.CircuitBreaker.Execute<WithdrawResult>(
            async (cts) => await service.Client.WithdrawCurrencyAsync(data, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result);
      }

      catch (CircuitOpenException) {
         return await WithdrawCurrency(data);
      }
   }

   [SwaggerOperation(Summary = "Retrieve the transaction history for a specified period")]
   [SwaggerResponse(StatusCodes.Status200OK, "Transaction history retrieved successfully.",
      typeof(TransactionsHistory))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid history request data.")]
   [HttpPost("History")]
   public async Task<ActionResult<TransactionsHistory>> GetHistory(GetHistoryOptions options) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(GetHistory)}] Load balancer gave {service.SdInstance}");

         TransactionsHistory result = await service.CircuitBreaker.Execute<TransactionsHistory>(
            async (cts) => await service.Client.GetHistoryAsync(options, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result);
      }

      catch (CircuitOpenException) {
         return await GetHistory(options);
      }
   }

   [SwaggerOperation(Summary = "Cancel or revert a transaction")]
   [SwaggerResponse(StatusCodes.Status200OK, "Transaction canceled successfully.", typeof(CancelTransactionResult))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid cancellation request data.")]
   [HttpPost("Cancel")]
   public async Task<ActionResult<CancelTransactionResult>> CancelTransaction(CancelTransactionOptions options) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await GetServiceAsync();

         logger.LogInformation($"[{nameof(CancelTransaction)}] Load balancer gave {service.SdInstance}");

         CancelTransactionResult result = await service.CircuitBreaker.Execute<CancelTransactionResult>(
            async (cts) => await service.Client.CancelTransactionAsync(options, cancellationToken: cts)
         );

         if (result.Error is not null) {
            return ServiceErrorHelper.ServiceErrorToActionResult(result.Error);
         }

         return Ok(result);
      }

      catch (CircuitOpenException) {
         return await CancelTransaction(options);
      }
   }
   
   private Task<ServiceWrapper<TransactionServiceClient>> GetServiceAsync() {
      return ServiceWrapper<TransactionServiceClient>.GetService(serviceDiscoveryService, ServiceNames.TransactionService);
   }
}
