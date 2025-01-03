using System.Text.Json;
using Gateway.Models;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Gateway.Services;

public class ServiceDiscoveryService(
   IHttpClientFactory httpClientFactory,
   IOptions<JsonOptions> jsonOptions,
   ILogger<ServiceDiscoveryService> logger
) {
   private static readonly string BaseUrl = Environment.GetEnvironmentVariable("SERVICE_DISCOVERY_URL")!;

   private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

   public async Task<RegistryEntry?> GetInstance(string serviceName) {
      HttpResponseMessage res = await _httpClient.GetAsync($"{BaseUrl}/Api/v1/load-balancing/{serviceName}");
      if (!res.IsSuccessStatusCode) {
         logger.LogError($"{res.StatusCode} {res.ReasonPhrase}");
         return null;
      }

      await using Stream stream = await res.Content.ReadAsStreamAsync();
      RegistryEntry? dto = await JsonSerializer.DeserializeAsync<RegistryEntry?>(
         stream,
         jsonOptions.Value.SerializerOptions
      );
      return dto;
   }

   public async Task RemoveInstance(string host) {
      await _httpClient.DeleteAsync($"{BaseUrl}/Api/v1/Registry/{host}");
   }

   public async Task<List<RegistryEntry>> GetInstanceUrls(string serviceName) {
      HttpResponseMessage res = await _httpClient.GetAsync($"{BaseUrl}/Api/v1/Registry/{serviceName}");
      res.EnsureSuccessStatusCode();

      await using Stream contentStream = await res.Content.ReadAsStreamAsync();

      var list = await JsonSerializer.DeserializeAsync<List<RegistryEntry>>(
         contentStream,
         jsonOptions.Value.SerializerOptions
      );

      return list!;
   }

   public async Task ShutdownService(string serviceUrl) {
      HttpResponseMessage res = await _httpClient.DeleteAsync($"{BaseUrl}/Api/v1/Registry/{serviceUrl}");
      res.EnsureSuccessStatusCode();
   }
}
