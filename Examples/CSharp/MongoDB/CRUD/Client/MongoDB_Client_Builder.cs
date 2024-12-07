using MongoDB.Driver;

namespace MongoDB.CRUD.Client;

public class MongoDB_Client_Builder : IMongoDB_Client_Builder
{
    public MongoDB_Client_Builder(MongoClientSettings mongoClientSettings)
    {
        MongoClientSettings = mongoClientSettings;
    }

    private MongoClientSettings MongoClientSettings { get; }

    public IMongoClient Build()
    {
        return new MongoClient(MongoClientSettings);
    }
}