package com.api.mongodb.rest.service.mongodb.crud;

import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import com.mongodb.client.model.Filters;

import org.bson.BsonDocument;
import org.bson.conversions.Bson;
import org.bson.types.ObjectId;
import java.util.concurrent.CompletableFuture;

public class MongoDB_CRUD_Client implements IMongoDB_CRUD_Client {
    static private String _connectionString;
    static private MongoClient _mongoClient;

    static public void initialize(String connectionString) {
        _connectionString = connectionString;
    }

    static private MongoClient getMongoClient() { 
        if (null == _mongoClient)
            _mongoClient = MongoClients.create(_connectionString);
        return _mongoClient; 
    }

    private MongoCollection<BsonDocument> getCollection(String dbName, String collectionName) {
        MongoDatabase database = getMongoClient().getDatabase(dbName);
        return database.getCollection(collectionName, BsonDocument.class);
    }

    public CompletableFuture<Void> ping() {
        return CompletableFuture.runAsync(() -> {
            getMongoClient().getDatabase("admin").runCommand(new BsonDocument("ping", new BsonDocument()));
        });
    }

    public CompletableFuture<ObjectId> createAsync(String dbName, String collectionName, BsonDocument document) {
        return CompletableFuture.supplyAsync(() -> {
            MongoCollection<BsonDocument> collection = getCollection(dbName, collectionName);
            collection.insertOne(document);
            return document.getObjectId("_id").getValue();
        });
    }

    public CompletableFuture<BsonDocument> readAsync(String dbName, String collectionName, ObjectId documentId) {
        return CompletableFuture.supplyAsync(() -> {
            MongoCollection<BsonDocument> collection = getCollection(dbName, collectionName);
            Bson filter = Filters.eq("_id", documentId);
            return collection.find(filter).first();
        });
    }

    public CompletableFuture<BsonDocument> updateAsync(String dbName, String collectionName, ObjectId documentId, BsonDocument updateFields) {
        return CompletableFuture.supplyAsync(() -> {
            MongoCollection<BsonDocument> collection = getCollection(dbName, collectionName);
            Bson filter = Filters.eq("_id", documentId);
            BsonDocument update = new BsonDocument("$set", updateFields);
            return collection.findOneAndUpdate(filter, update);
        });
    }

    public CompletableFuture<Boolean> deleteAsync(String dbName, String collectionName, ObjectId documentId) {
        return CompletableFuture.supplyAsync(() -> {
            MongoCollection<BsonDocument> collection = getCollection(dbName, collectionName);
            Bson filter = Filters.eq("_id", documentId);
            return collection.deleteOne(filter).getDeletedCount() == 1;
        });
    }    
}