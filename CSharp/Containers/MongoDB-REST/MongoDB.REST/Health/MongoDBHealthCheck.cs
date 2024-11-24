using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.CRUD.Client;
using MongoDB.REST.Client;

namespace MongoDB.REST.Health;

public class MongoDBHealthCheck : IHealthCheck
{
    private readonly IMongoDB_Client_Builder _mongoDB_Client_Builder;

    public MongoDBHealthCheck(IMongoDB_Client_Builder mongoDB_Client_Builder)
    {
        _mongoDB_Client_Builder = mongoDB_Client_Builder;
    }

    private MongoDB_REST_Client CreateMongoDBClient()
    {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client(_mongoDB_Client_Builder));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await CreateMongoDBClient().Ping();
            return HealthCheckResult.Healthy("The MongoDB database is reachable.");
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckResult.Unhealthy("The API is unreachable.", ex);
        }
    }
}
