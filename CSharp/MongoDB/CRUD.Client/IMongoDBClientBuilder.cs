using MongoDB.Driver;

namespace MongoDB.CRUD;

public interface IMongoDBClientBuilder
{
    IMongoClient Build();
}