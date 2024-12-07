using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client; // Add this at the top of the file
using MongoDB.Driver;
using MongoDB.REST.Health;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB configuration
var config = builder.Configuration;
var mongoDbSection = config.GetSection("MongoDB");
var mongoDbServer = Environment.GetEnvironmentVariable("MONGODB_SERVER") ?? mongoDbSection.GetValue<string>("MONGODB_SERVER");
var mongoDbUsername = Environment.GetEnvironmentVariable("MONGODB_USERNAME") ?? mongoDbSection.GetValue<string>("MONGODB_USERNAME");
var mongoDbPassword = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? mongoDbSection.GetValue<string>("MONGODB_PASSWORD");
var mongoDbAppName = mongoDbSection.GetValue<string>("appName");
var mongoDbRetryWrites = mongoDbSection.GetValue<bool>("retryWrites");
var mongoDbWriteConcern = mongoDbSection.GetValue<string>("w");
var mongodb_connection_string = $"mongodb+srv://{mongoDbUsername}:{mongoDbPassword}@{mongoDbServer}/" + 
        $"?retryWrites={mongoDbRetryWrites}&w={mongoDbWriteConcern}&appName={mongoDbAppName}";
// Dependency injection
builder.Services.AddSingleton(sp => MongoClientSettings.FromConnectionString(mongodb_connection_string));
builder.Services.AddScoped<IMongoDB_Client_Builder, MongoDB_Client_Builder>();
// Add controllers
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<MongoDBHealthCheck>("mongodb")
    ;

// Configure OpenTelemetry tracing
var sourceName = "csharp-mongodb-rest-api";
var serviceName = sourceName;
var otlp_endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
var otlp_api_key = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_API_KEY");
var loki_url = Environment.GetEnvironmentVariable("LOKI_URL");
var loki_api_key = Environment.GetEnvironmentVariable("LOKI_API_KEY");
Console.WriteLine($"OTLP Endpoint: {otlp_endpoint}");
Console.WriteLine($"OTLP API Key: {otlp_api_key}");
Console.WriteLine($"Loki URL: {loki_url}");
Console.WriteLine($"Loki API Key: {loki_api_key}");

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(sourceName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri($"{otlp_endpoint}/v1/traces");
                options.Headers = $"Authorization=Bearer {otlp_api_key}";
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    })
    .WithMetrics(meterProviderBuilder =>
    {
        meterProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri($"{otlp_endpoint}/v1/metrics");
                options.Headers = $"Authorization=Bearer {otlp_api_key}";
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    });

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(loki_url!, credentials: new LokiCredentials
        {
            Password = loki_api_key!,
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
// Map health checks endpoint
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    ResponseWriter = async (context, report) => await context.Response.WriteAsJsonAsync(new HealthResults(report))
});
app.MapGet("/", () =>
{
    var meter = new Meter(sourceName);
    var counter = meter.CreateCounter<int>("custom_metric_counter");
    counter.Add(1);
    Log.Information("Hello from .NET application!");
    return "Hello, Grafana Loki with Serilog!";
});
app.Run();
