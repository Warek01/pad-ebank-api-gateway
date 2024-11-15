using Asp.Versioning;
using Gateway.ExceptionHandlers;
using Gateway.Services;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using StackExchange.Redis;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration)
   .Enrich.FromLogContext()
   .CreateLogger();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
   options.SwaggerDoc("v1", new OpenApiInfo {
      Title = "PAD API Gateway",
      Description = "Gateway microservice",
      Version = "v1",
   });
   options.EnableAnnotations();

   var filePath = Path.Combine(AppContext.BaseDirectory, "Gateway.xml");
   options.IncludeXmlComments(filePath);
});
builder.Services.UseHttpClientMetrics();
builder.Services.AddHttpClient();
builder.Services.AddApiVersioning(options => {
   options.ReportApiVersions = true;
   options.AssumeDefaultVersionWhenUnspecified = true;
   options.DefaultApiVersion = new ApiVersion(1);
   options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options => {
   options.GroupNameFormat = "'v'V";
   options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSerilog();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ServiceUnavailableExceptionHandler>();
builder.Services.AddHealthChecks();
LoadServices();
SetupRedis();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger(options => { options.RouteTemplate = "Api/Docs/{documentName}/swagger.json"; });
app.UseSwaggerUI(options => {
   options.SwaggerEndpoint("/Api/Docs/v1/swagger.json", "Gateway v1");
   options.DocumentTitle = "Gateway docs";
   options.RoutePrefix = "Api/Docs";
});
app.UseMetricServer();
app.UseHttpMetrics();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHealthChecks("/Healthz");
app.MapControllers();

Run();

return;

void Run() {
   string scheme = Environment.GetEnvironmentVariable("HTTP_SCHEME")!;
   string host = Environment.GetEnvironmentVariable("HTTP_HOST")!;
   string port = Environment.GetEnvironmentVariable("HTTP_PORT")!;
   string url = $"{scheme}://{host}:{port}";

   app.Run(url);
}

void LoadServices() {
   builder.Services.AddScoped<ServiceDiscoveryService>();
   builder.Services.AddScoped<SagaOrchestratorService>();
   builder.Services.AddScoped<CacheService>();
}

void SetupRedis() {
   int db = int.Parse(Environment.GetEnvironmentVariable("REDIS_DB")!);
   string user = Environment.GetEnvironmentVariable("REDIS_USER")!;
   string password = Environment.GetEnvironmentVariable("REDIS_PASSWORD")!;

   var options = new ConfigurationOptions {
      Protocol = RedisProtocol.Resp3,
      EndPoints = {
         "redis-node-1:6379", "redis-node-2:6379", "redis-node-3:6379", "redis-node-4:6379", "redis-node-5:6379",
         "redis-node-6:6379"
      },
      Password = password,
      User = user,
      DefaultDatabase = db,
      Ssl = false,
      AllowAdmin = false,
      ConnectRetry = 3,
      ConnectTimeout = 5000,
   };

   builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));
}
