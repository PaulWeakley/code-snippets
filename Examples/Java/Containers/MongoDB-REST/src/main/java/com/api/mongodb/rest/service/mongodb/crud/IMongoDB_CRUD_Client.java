package com.api.mongodb.rest.service.mongodb.crud;

import org.bson.BsonDocument;
import org.bson.types.ObjectId;
import java.util.concurrent.CompletableFuture;

public interface IMongoDB_CRUD_Client {
    CompletableFuture<Void> ping();
    CompletableFuture<ObjectId> createAsync(String dbName, String collectionName, BsonDocument document);
    CompletableFuture<BsonDocument> readAsync(String dbName, String collectionName, ObjectId documentId);
    CompletableFuture<BsonDocument> updateAsync(String dbName, String collectionName, ObjectId documentId, BsonDocument updateFields);
    CompletableFuture<Boolean> deleteAsync(String dbName, String collectionName, ObjectId documentId);
}