using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDB.CRUD.Client;

public interface IMongoDB_CRUD_Client : IDisposable
{
    Task Ping();
    Task<ObjectId?> CreateAsync(string dbName, string collectionName, BsonDocument document, CancellationToken cancellationToken = default);
    Task<BsonDocument?> ReadAsync(string dbName, string collectionName, ObjectId documentId, CancellationToken cancellationToken = default);
    Task<BsonDocument?> UpdateAsync(string dbName, string collectionName, ObjectId documentId, BsonDocument updateFields, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string dbName, string collectionName, ObjectId documentId, CancellationToken cancellationToken = default);
}