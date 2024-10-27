using Asp.Versioning;
using Gateway.ExceptionHandlers;
using Gateway.Services;
using Microsoft.OpenApi.Models;
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
      Version = "v1",
   });
   options.EnableAnnotations();

   var filePath = Path.Combine(AppContext.BaseDirectory, "Gateway.xml");
   options.IncludeXmlComments(filePath);
});
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
app.UseSwagger();
app.UseSwaggerUI();
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
   builder.Services.AddSingleton<ServiceDiscoveryService>();
   builder.Services.AddSingleton<AccountServiceLoadBalancer>();
   builder.Services.AddSingleton<TransactionServiceLoadBalancer>();
}

void SetupRedis() {
   string host = Environment.GetEnvironmentVariable("REDIS_HOST")!;
   string port = Environment.GetEnvironmentVariable("REDIS_PORT")!;
   int db = int.Parse(Environment.GetEnvironmentVariable("REDIS_DB")!);
   string user = Environment.GetEnvironmentVariable("REDIS_USER")!;
   string password = Environment.GetEnvironmentVariable("REDIS_PASSWORD")!;

   var options = new ConfigurationOptions {
      Protocol = RedisProtocol.Resp3,
      EndPoints = { $"{host}:{port}" },
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
