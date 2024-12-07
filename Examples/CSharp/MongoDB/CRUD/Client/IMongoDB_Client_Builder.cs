using MongoDB.Driver;

namespace MongoDB.CRUD.Client;

public interface IMongoDB_Client_Builder
{
    IMongoClient Build();
}