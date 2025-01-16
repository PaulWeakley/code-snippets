using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client; // Add this at the top of the file
using MongoDB.REST.Client;
using MongoDB.Driver;
using MongoDB.REST.Health;
using System.Text;
using MongoDB.Bson;
using System.Text.Json;
using MongoDB.REST;

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

app.MapPost("api/telemetry/{raw_start}", async (IMongoClient mongoClient, HttpContext context, double raw_start) =>
{
    DateTime start = DateTimeOffset.FromUnixTimeSeconds((long)raw_start).UtcDateTime
            .AddMilliseconds((raw_start % 1) * 1000);
    var containerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
    TimeSpan sslHandshake = DateTime.UtcNow - start;
    var start_ref = DateTime.UtcNow;

    var measure = DateTime.UtcNow;
    using var reader = new StreamReader(context.Request.Body);
    var body = BsonDocument.Parse(await reader.ReadToEndAsync());
    TimeSpan deserialization = DateTime.UtcNow - measure;
    
    var db_name = "test";
    var collection_name = "source";
    measure = DateTime.UtcNow;
    var client = new MongoDB_CRUD_Client(mongoClient);
    var document_id = await client.CreateAsync(db_name, collection_name, body);
    var createObject = DateTime.UtcNow - measure;
    
    measure = DateTime.UtcNow;
    var document = await client.ReadAsync(db_name, collection_name, document_id.Value);
    var getObject = DateTime.UtcNow - measure;

    measure = DateTime.UtcNow;
    var jsonData = document.ToJson();
    TimeSpan serialization = DateTime.UtcNow - measure;

    // High IOps operation
    measure = DateTime.UtcNow;
    for (int i = 0; i < 15; i++)
        await client.UpdateAsync(db_name, collection_name, document_id.Value, new BsonDocument("time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
    TimeSpan highIOps = DateTime.UtcNow - measure;

    // Delete the document
    measure = DateTime.UtcNow;
    await client.DeleteAsync(db_name, collection_name, document_id.Value);
    TimeSpan deleteObject = DateTime.UtcNow - measure;
    
    TimeSpan total = DateTime.UtcNow - start_ref;

    var telemetry = new Telemetry
    {
            Start = raw_start,
            ContainerTime = containerTime,
            SslHandshake = sslHandshake.TotalMilliseconds,
            Deserialization = deserialization.TotalMilliseconds,
            CreateObject = createObject.TotalMilliseconds,
            GetObject = getObject.TotalMilliseconds,
            Serialization = serialization.TotalMilliseconds,
            HighIOps = highIOps.TotalMilliseconds,
            deleteObject = deleteObject.TotalMilliseconds,
            Total = total.TotalMilliseconds
    };

    var jsonTypeInfo = TelemetryJsonContext.Default.GetTypeInfo(telemetry.GetType());
    return Results.Json(telemetry, jsonTypeInfo);
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

[JsonSerializable(typeof(Telemetry))]
public partial class TelemetryJsonContext : JsonSerializerContext{}