using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Gateway.Services;

public class CacheService(IConnectionMultiplexer connMuxer, IOptions<JsonOptions> jsonOptions) {
   private static class CacheKeys {
      public const string TransactionsHistory = "Gateway/Transaction/History";
   }

   private readonly IDatabase _database = connMuxer.GetDatabase();
   private readonly JsonSerializerOptions _serializerOptions = jsonOptions.Value.SerializerOptions;

   public async Task<List<Transaction>?> GetTransactionsHistoryAsync(GetHistoryOptions options) {
      string key = GetTransactionHistoryKey(options);
      string? value = await _database.HashGetAsync(CacheKeys.TransactionsHistory, key);

      if (value is null) {
         return null;
      }

      byte[] bytes = Encoding.UTF8.GetBytes(value);
      await using Stream stream = new MemoryStream(bytes);
      return await JsonSerializer.DeserializeAsync<List<Transaction>>(stream, _serializerOptions);
   }

   public async Task SetTransactionsHistory(GetHistoryOptions options, List<Transaction> transactions) {
      string key = GetTransactionHistoryKey(options);
      await using var stream = new MemoryStream();
      await JsonSerializer.SerializeAsync(stream, transactions, _serializerOptions);
      await _database.HashSetAsync(CacheKeys.TransactionsHistory, key, stream.ToArray());
   }

   private string GetTransactionHistoryKey(GetHistoryOptions options) {
      return $"{options.CardCode}/{options.Year}/{options.Month}";
   }
}
