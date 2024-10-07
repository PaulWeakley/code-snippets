using MongoDB.Driver;

namespace MongoDB.CRUD;

public class MongoDBClientBuilder : IMongoDBClientBuilder
{
    public MongoDBClientBuilder(MongoClientSettings mongoClientSettings)
    {
        MongoClientSettings = mongoClientSettings;
    }

    private MongoClientSettings MongoClientSettings { get; }

    public IMongoClient Build()
    {
        return new MongoClient(MongoClientSettings);
    }
}