using System.Text.Json;
using Gateway.Dtos.Request;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace Gateway.Services;

public class ServiceDiscoveryService(
  IHttpClientFactory httpClientFactory,
  IOptions<JsonOptions> jsonOptions
) {
  private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
  private readonly string _baseUrl = Environment.GetEnvironmentVariable("SERVICE_DISCOVERY_URL")!;

  public async Task<List<ServiceInstanceDto>> GetInstanceUrls(string serviceName) {
    HttpResponseMessage res = await _httpClient.GetAsync($"{_baseUrl}/Api/v1/Registry/{serviceName}");
    res.EnsureSuccessStatusCode();

    await using Stream contentStream = await res.Content.ReadAsStreamAsync();

    var list = await JsonSerializer.DeserializeAsync<List<ServiceInstanceDto>>(
      contentStream,
      jsonOptions.Value.SerializerOptions
    );

    return list!;
  }

  // public async Task<List<InstanceDto>> GetAllInstancesUrls() {
  //   HttpResponseMessage res = await _httpClient.GetAsync($"{_baseUrl}/Api/v1/Registry/");
  //   res.EnsureSuccessStatusCode();
  //
  //   return await JsonSerializer.DeserializeAsync<List<string>>(await res.Content.ReadAsStreamAsync()) ?? [];
  // }

  public async Task ShutdownService(string serviceUrl) {
    HttpResponseMessage res = await _httpClient.DeleteAsync($"{_baseUrl}/Api/v1/Registry/{serviceUrl}");
    res.EnsureSuccessStatusCode();
  }
}
