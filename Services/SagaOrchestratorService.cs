using Gateway.Exceptions;
using Gateway.Helpers;
using Gateway.Models;

namespace Gateway.Services;

public class SagaOrchestratorService(
   ILogger<SagaOrchestratorService> logger,
   ServiceDiscoveryService sd
) {
   public async Task<ServiceError?> DepositCurrencyAsync(DepositData data) {
      AddCurrencyResult srcCardSubtractResult = await AddCurrencyAsync(new AddCurrencyOptions {
         Amount = data.Amount,
         Currency = data.Currency,
         CardCode = data.CardCode,
      });

      if (srcCardSubtractResult.Error is not null) {
         logger.LogError("Error depositing money");

         return srcCardSubtractResult.Error;
      }
      
      
      CreateTransactionResult createTransactionResult = await CreateTransactionAsync(
         new CreateTransactionData {
            Type = TransactionType.Transfer,
            DepositData = data,
         });

      if (createTransactionResult.Error is not null) {
         logger.LogError("Error creating transaction");

         await AddCurrencyAsync(new AddCurrencyOptions {
            Amount = -data.Amount,
            Currency = data.Currency,
            CardCode = data.CardCode,
         });

         return createTransactionResult.Error;
      }

      return null;
   }
   public async Task<ServiceError?> WithdrawCurrencyAsync(WithdrawData data) {
      AddCurrencyResult srcCardSubtractResult = await AddCurrencyAsync(new AddCurrencyOptions {
         Amount = -data.Amount,
         Currency = data.Currency,
         CardCode = data.CardCode,
      });

      if (srcCardSubtractResult.Error is not null) {
         logger.LogError("Error depositing money");

         return srcCardSubtractResult.Error;
      }
      
      
      CreateTransactionResult createTransactionResult = await CreateTransactionAsync(
         new CreateTransactionData {
            Type = TransactionType.Transfer,
            WithdrawData = data,
         });

      if (createTransactionResult.Error is not null) {
         logger.LogError("Error creating transaction");

         await AddCurrencyAsync(new AddCurrencyOptions {
            Amount = data.Amount,
            Currency = data.Currency,
            CardCode = data.CardCode,
         });

         return createTransactionResult.Error;
      }

      return null;
   }
   
   public async Task<ServiceError?> TransferCurrencyAsync(TransferData data) {
      AddCurrencyResult srcCardSubtractResult = await AddCurrencyAsync(new AddCurrencyOptions {
         Amount = -data.Amount,
         Currency = data.Currency,
         CardCode = data.SrcCardCode,
      });

      if (srcCardSubtractResult.Error is not null) {
         logger.LogError("Error subtracting money from src card");

         return srcCardSubtractResult.Error;
      }

      AddCurrencyResult dstCardAddResult = await AddCurrencyAsync(new AddCurrencyOptions {
         Amount = data.Amount,
         Currency = data.Currency,
         CardCode = data.DstCardCode,
      });

      if (dstCardAddResult.Error is not null) {
         logger.LogError("Error adding money to destination card");

         await AddCurrencyAsync(new AddCurrencyOptions {
            Amount = data.Amount,
            Currency = data.Currency,
            CardCode = data.SrcCardCode,
         });

         return dstCardAddResult.Error;
      }

      CreateTransactionResult createTransactionResult = await CreateTransactionAsync(
         new CreateTransactionData {
            Type = TransactionType.Transfer,
            TransferData = data,
         });

      if (createTransactionResult.Error is not null) {
         logger.LogError("Error creating transaction");

         await AddCurrencyAsync(new AddCurrencyOptions {
            Amount = data.Amount,
            Currency = data.Currency,
            CardCode = data.SrcCardCode,
         });

         await AddCurrencyAsync(new AddCurrencyOptions {
            Amount = -data.Amount,
            Currency = data.Currency,
            CardCode = data.DstCardCode,
         });

         return createTransactionResult.Error;
      }

      return null;
   }

   private async Task<AddCurrencyResult> AddCurrencyAsync(AddCurrencyOptions options) {
      var accountService = await ServiceWrapper<AccountServiceClient>.GetAccountServiceAsync(sd);

      try {
         AddCurrencyResult result = await accountService.CircuitBreaker.Execute<AddCurrencyResult>(
            async (cts) => await accountService.Client.AddCurrencyAsync(options, cancellationToken: cts)
         );

         return result;
      }
      catch (CircuitOpenException) {
         return await AddCurrencyAsync(options);
      }
   }

   private async Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionData data) {
      var transactionService = await ServiceWrapper<TransactionServiceClient>.GetTransactionServiceAsync(sd);


      try {
         CreateTransactionResult result = await transactionService.CircuitBreaker.Execute<CreateTransactionResult>(
            async (cts) => await transactionService.Client.CreateTransactionAsync(data, cancellationToken: cts)
         );

         return result;
      }
      catch (CircuitOpenException) {
         return await CreateTransactionAsync(data);
      }
   }
}
