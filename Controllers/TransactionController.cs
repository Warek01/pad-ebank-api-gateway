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
public class TransactionController(
   TransactionServiceLoadBalancer loadBalancer,
   ILogger<TransactionController> logger
) : ControllerBase {
   [HttpPost("Currency/Transfer")]
   public async Task<ActionResult<TransferResult>> TransferCurrency(TransferData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(TransferCurrency)}] Load balancer gave {service.InstanceDto}");

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

   [HttpPost("Currency/Deposit")]
   public async Task<ActionResult<DepositResult>> DepositCurrency(DepositData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(DepositCurrency)}] Load balancer gave {service.InstanceDto}");

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

   [HttpPost("Currency/Withdraw")]
   public async Task<ActionResult<WithdrawResult>> WithdrawCurrency(WithdrawData data) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(WithdrawCurrency)}] Load balancer gave {service.InstanceDto}");

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

   [HttpPost("History")]
   public async Task<ActionResult<TransactionsHistory>> GetHistory(GetHistoryOptions options) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(GetHistory)}] Load balancer gave {service.InstanceDto}");

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

   [HttpPost("Cancel")]
   public async Task<ActionResult<CancelTransactionResult>> CancelTransaction(CancelTransactionOptions options) {
      try {
         ServiceWrapper<TransactionServiceClient> service = await loadBalancer.GetServiceAsync();

         logger.LogInformation($"[{nameof(CancelTransaction)}] Load balancer gave {service.InstanceDto}");

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
}
