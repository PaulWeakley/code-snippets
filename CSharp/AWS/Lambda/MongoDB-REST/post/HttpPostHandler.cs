using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using MongoDB.CRUD;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Core.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MongoDB.REST
{
    public class HttpPostHandler
    {
        /// <summary>
        /// Function handler to process HTTP requests via API Gateway.
        /// </summary>
        /// <param name="request">The API Gateway request</param>
        /// <param name="context">The Lambda context</param>
        /// <returns>API Gateway response</returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            // Log the request
            context.Logger.LogLine("Received request: " + request.Body);

            if (!request.PathParameters.TryGetValue("db_name", out string? dbName) || string.IsNullOrEmpty(dbName))
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Error: Missing db_name parameter"
                };

            if (!request.PathParameters.TryGetValue("collection_name", out string? collectionName) || string.IsNullOrEmpty(collectionName))
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Error: Missing collection_name parameter"
                };

            var config = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            if (string.IsNullOrEmpty(request.Body))
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Error: Missing body"
                };

            var mongoDbSection = config.GetSection("MongoDB");
            var mongoDbServer = Environment.GetEnvironmentVariable("MONGODB_SERVER") ?? mongoDbSection.GetValue<string>("MONGODB_SERVER");
            var mongoDbAuthDb = Environment.GetEnvironmentVariable("MONGODB_AUTH_DB") ?? mongoDbSection.GetValue<string>("MONGODB_AUTH_DB");
            var mongoDbUsername = Environment.GetEnvironmentVariable("MONGODB_USERNAME") ?? mongoDbSection.GetValue<string>("MONGODB_USERNAME");
            var mongoDbPassword = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") ?? mongoDbSection.GetValue<string>("MONGODB_PASSWORD");
            var mongoDbAppName = Environment.GetEnvironmentVariable("MONGODB_APP_NAME") ?? mongoDbSection.GetValue<string>("MONGODB_APP_NAME");
            var mongoDbRetryWrites = Environment.GetEnvironmentVariable("MONGODB_RETRY_WRITES") ?? mongoDbSection.GetValue<string>("MONGODB_RETRY_WRITES");
            var mongoDbWriteConcern = Environment.GetEnvironmentVariable("MONGODB_WRITE_CONCERN") ?? mongoDbSection.GetValue<string>("MONGODB_WRITE_CONCERN");

            var mongoDbSettings = MongoClientSettings.FromConnectionString(
                $"mongodb+srv://{mongoDbUsername}:{mongoDbPassword}@{mongoDbServer}/" + 
                $"?retryWrites={mongoDbRetryWrites}&w={mongoDbWriteConcern}&appName={mongoDbAppName}");

            var mongoDBClientBuilder = new MongoDBClientBuilder(mongoDbSettings);
            
            var documentId = await new MongoDBCrudClient(mongoDBClientBuilder)
                .CreateAsync(dbName, collectionName, BsonDocument.Parse(request.Body));

            // Return the API Gateway response
            return new APIGatewayProxyResponse {
                StatusCode = 200,
                Body = documentId.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
