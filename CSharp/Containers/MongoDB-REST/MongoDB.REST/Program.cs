using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client; // Add this at the top of the file
using MongoDB.Driver;
using MongoDB.REST.Health;

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
app.Run();
