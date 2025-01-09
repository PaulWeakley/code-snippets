using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client; // Add this at the top of the file
using MongoDB.REST.Client;
using MongoDB.Driver;
using MongoDB.REST.Health;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// MongoDB configuration
var mongodb_server = Environment.GetEnvironmentVariable("MONGODB_SERVER") ?? "localhost";
var mongodb_username = Environment.GetEnvironmentVariable("MONGODB_USERNAME") ?? "root";
var mongodb_password = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? "password";
var mongodb_retry_writes = Environment.GetEnvironmentVariable("MONGODB_RETRY_WRITES") ?? "true";
var mongodb_write_concern = Environment.GetEnvironmentVariable("MONGODB_WRITE_CONCERN") ?? "majority";
var mongodb_app_name = Environment.GetEnvironmentVariable("MONGODB_APP_NAME") ?? "MongoDB.REST";
var mongodb_min_pool_size = Environment.GetEnvironmentVariable("MONGODB_MIN_POOL_SIZE") ?? "10";
var mongodb_max_pool_size = Environment.GetEnvironmentVariable("MONGODB_MAX_POOL_SIZE") ?? "50";
var mongodb_wait_queue_size = Environment.GetEnvironmentVariable("MONGODB_WAIT_QUEUE_SIZE") ?? "1000";

var mongodb_connection_string = $"mongodb+srv://{mongodb_username}:{mongodb_password}@{mongodb_server}/" + 
        $"?retryWrites={mongodb_retry_writes}&w={mongodb_write_concern}&appName={mongodb_app_name}" + 
        $"&minPoolSize={mongodb_min_pool_size}&maxPoolSize={mongodb_max_pool_size}&waitQueueSize={mongodb_wait_queue_size}";
// Dependency injection
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(MongoClientSettings.FromConnectionString(mongodb_connection_string)));
// Add controllers
builder.Services.AddHealthChecks()
    .AddCheck<MongoDBHealthCheck>("mongodb")
    ;
Console.WriteLine($"My native AOT application is starting.....");
var app = builder.Build();

static IResult ToActionResult(REST_Response response)
{
    return Results.Content(response.Body, response.ContentType, Encoding.UTF8, response.StatusCode);
}

MongoDB_REST_Client CreateMongoDBClient(IMongoClient mongoClient)
{
    return new MongoDB_REST_Client(new MongoDB_CRUD_Client(mongoClient));
}

app.MapGet("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoClient mongoClient, string db_name, string collection_name, string id) =>
{
    var client = CreateMongoDBClient(mongoClient);
    var response = await client.GetAsync(db_name, collection_name, id);
    return ToActionResult(response);
});

app.MapPost("api/mongodb/{db_name}/{collection_name}", async (IMongoClient mongoClient, HttpContext context, string db_name, string collection_name) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoClient);
    var response = await client.PostAsync(db_name, collection_name, body);
    return ToActionResult(response);
});

app.MapPut("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoClient mongoClient, HttpContext context, string db_name, string collection_name, string id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoClient);
    var response = await client.PutAsync(db_name, collection_name, id, body);
    return ToActionResult(response);
});

app.MapPatch("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoClient mongoClient, HttpContext context, string db_name, string collection_name, string id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoClient);
    var response = await client.PutAsync(db_name, collection_name, id, body);
    return ToActionResult(response);
});

app.MapDelete("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoClient mongoClient, string db_name, string collection_name, string id) =>
{
    var client = CreateMongoDBClient(mongoClient);
    var response = await client.DeleteAsync(db_name, collection_name, id);
    return ToActionResult(response);
});

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
// Map health checks endpoint
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    ResponseWriter = async (context, report) => 
    {
        var jsonTypeInfo = HealthResultsJsonContext.Default.HealthResults;
        await context.Response.WriteAsJsonAsync(new HealthResults(report), jsonTypeInfo);
    }
});
app.Run();

[JsonSerializable(typeof(HealthResults))]
public partial class HealthResultsJsonContext : JsonSerializerContext{}
