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
   SagaOrchestratorService orchestrator,
   ServiceDiscoveryService sd
) : ControllerBase {
   [SwaggerOperation(Summary = "Transfer money from one account to another")]
   [SwaggerResponse(StatusCodes.Status200OK, "Transfer completed successfully.", typeof(Nullable))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid transfer data.")]
   [HttpPost("Currency/Transfer")]
   public async Task<ActionResult<ServiceError?>> TransferCurrency(TransferData data) {
      return await orchestrator.TransferCurrencyAsync(data);
   }

   [SwaggerOperation(Summary = "Deposit money into a card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Deposit completed successfully.", typeof(Nullable))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid deposit data.")]
   [HttpPost("Currency/Deposit")]
   public async Task<ActionResult<ServiceError?>> DepositCurrency(DepositData data) {
      return await orchestrator.DepositCurrencyAsync(data);
   }

   [SwaggerOperation(Summary = "Withdraw money from a card")]
   [SwaggerResponse(StatusCodes.Status200OK, "Withdrawal completed successfully.", typeof(Nullable))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid withdrawal data.")]
   [HttpPost("Currency/Withdraw")]
   public async Task<ActionResult<ServiceError?>> WithdrawCurrency(WithdrawData data) {
      return await orchestrator.WithdrawCurrencyAsync(data);
   }

   [SwaggerOperation(Summary = "Retrieve the transaction history for a specified period")]
   [SwaggerResponse(StatusCodes.Status200OK, "Transaction history retrieved successfully.",
      typeof(TransactionsHistory))]
   [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid history request data.")]
   [HttpPost("History")]
   public async Task<ActionResult<TransactionsHistory>> GetHistory(GetHistoryOptions options) {
      try {
         var service = await ServiceWrapper<TransactionServiceClient>.GetTransactionServiceAsync(sd);

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
}
