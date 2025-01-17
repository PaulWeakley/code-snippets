﻿namespace MongoDB.CRUD.Client;

using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDB_CRUD_Client(IMongoDB_Client_Builder mongoDBClientBuilder) : IMongoDB_CRUD_Client
{
    private IMongoDB_Client_Builder MongoDBClientBuilder { get; } = mongoDBClientBuilder;
    private IMongoClient? MongoClient { get; set; }

    private IMongoClient GetClient()
    {
        if (null == MongoClient)
            MongoClient = MongoDBClientBuilder.Build();
        return MongoClient;
    }

    public async Task Ping()
    {
        await GetClient().GetDatabase("admin").RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    }

    private IMongoCollection<BsonDocument> GetCollection(string dbName, string collectionName)
    {
        var client = GetClient();
        var database = client.GetDatabase(dbName);
        return database.GetCollection<BsonDocument>(collectionName);
    }

    public async Task<ObjectId?> CreateAsync(string dbName, string collectionName, BsonDocument document, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(dbName, collectionName);
        await collection.InsertOneAsync(document, null, cancellationToken);
        return document["_id"].AsObjectId;
    }

    public async Task<BsonDocument?> ReadAsync(string dbName, string collectionName, ObjectId documentId, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(dbName, collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", documentId);
        return (await collection.FindAsync(filter, null, cancellationToken)).FirstOrDefault(cancellationToken);        
    }

    public async Task<BsonDocument?> UpdateAsync(string dbName, string collectionName, ObjectId documentId, BsonDocument updateFields, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(dbName, collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", documentId);
        var update = Builders<BsonDocument>.Update
                    .Combine(updateFields.Elements.Select(e => Builders<BsonDocument>.Update.Set(e.Name, e.Value)));
        return await collection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<BsonDocument> { ReturnDocument = ReturnDocument.After }, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string dbName, string collectionName, ObjectId documentId, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection(dbName, collectionName);
        var filter = Builders<BsonDocument>.Filter.Eq("_id", documentId);
        var result = await collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount == 1;
    }
}