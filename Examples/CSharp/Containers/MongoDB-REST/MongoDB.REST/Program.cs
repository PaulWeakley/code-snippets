using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client; // Add this at the top of the file
using MongoDB.REST.Client;
using MongoDB.Driver;
using MongoDB.REST.Health;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddHealthChecks()
    .AddCheck<MongoDBHealthCheck>("mongodb")
    ;
Console.WriteLine($"My native AOT application is starting.....");
var app = builder.Build();

static IResult ToActionResult(REST_Response response)
{
    return Results.Content(response.Body, response.ContentType, Encoding.UTF8, response.StatusCode);
}

MongoDB_REST_Client CreateMongoDBClient(IMongoDB_Client_Builder mongoDB_Client_Builder)
{
    return new MongoDB_REST_Client(new MongoDB_CRUD_Client(mongoDB_Client_Builder));
}

app.MapGet("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoDB_Client_Builder mongoDB_Client_Builder, string db_name, string collection_name, string id) =>
{
    var client = CreateMongoDBClient(mongoDB_Client_Builder);
    var response = await client.GetAsync(db_name, collection_name, id);
    return ToActionResult(response);
});

app.MapPost("api/mongodb/{db_name}/{collection_name}", async (IMongoDB_Client_Builder mongoDB_Client_Builder, HttpContext context, string db_name, string collection_name) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoDB_Client_Builder);
    var response = await client.PostAsync(db_name, collection_name, body);
    return ToActionResult(response);
});

app.MapPut("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoDB_Client_Builder mongoDB_Client_Builder, HttpContext context, string db_name, string collection_name, string id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoDB_Client_Builder);
    var response = await client.PutAsync(db_name, collection_name, id, body);
    return ToActionResult(response);
});

app.MapPatch("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoDB_Client_Builder mongoDB_Client_Builder, HttpContext context, string db_name, string collection_name, string id) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var client = CreateMongoDBClient(mongoDB_Client_Builder);
    var response = await client.PutAsync(db_name, collection_name, id, body);
    return ToActionResult(response);
});

app.MapDelete("api/mongodb/{db_name}/{collection_name}/{id}", async (IMongoDB_Client_Builder mongoDB_Client_Builder, string db_name, string collection_name, string id) =>
{
    var client = CreateMongoDBClient(mongoDB_Client_Builder);
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
