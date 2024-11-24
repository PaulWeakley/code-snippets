package com.api.mongodb.rest.service.mongodb.rest;


import com.api.mongodb.rest.service.mongodb.crud.IMongoDB_CRUD_Client;
import java.util.concurrent.CompletableFuture;
import org.bson.json.JsonMode;
import org.bson.json.JsonWriterSettings;
import org.bson.BsonDocument;
import org.bson.BsonString;
import org.bson.BsonValue;
import org.bson.types.ObjectId;

public class MongoDB_REST_Client
{
    private final IMongoDB_CRUD_Client __crud_client;

    public MongoDB_REST_Client(IMongoDB_CRUD_Client mongodb_crud_client)
    {
        this.__crud_client = mongodb_crud_client;
    }

    private static JsonWriterSettings jsonWriterSettings = JsonWriterSettings.builder().outputMode(JsonMode.RELAXED).build();

    private static REST_Response badRequest(String message){
        return new REST_Response(400, "text/plain", message);
    }

    private static REST_Response errorMessage(Throwable e) {
        return errorMessage(e.getMessage());
    }

    private static REST_Response errorMessage(String message) {
        return new REST_Response(500, "text/plain", message);
    }

    private static REST_Response notFound(ObjectId id) {
        return new REST_Response(404, "text/plain", "Exception: Document with id " + id + " not found");
    }

    private static REST_Response ok(BsonDocument data) {
        if (data.containsKey("_id")) {
            ObjectId id = data.getObjectId("_id").getValue();
            data.put("_id", new BsonString(id.toString()));
        } else {
            System.out.println("No _id found in document");
        }
        return new REST_Response(200, "application/json", data.toJson(jsonWriterSettings));
    }

    private static REST_Response created(ObjectId id) {
        return new REST_Response(201, "text/plain", id.toString());
    }

    private static REST_Response deleted(String id) {
        return new REST_Response(200, "text/plain", "Document with id " + id + " deleted");
    }

    private static REST_Response verifyParameters(String dbName, String collectionName, String id, boolean isIdRequired, Object data, boolean isDataRequired)
    {
        if (dbName == null || dbName.isEmpty())
            return badRequest("Exception: Missing db_name parameter");
        
        if (collectionName == null || collectionName.isEmpty())
            return badRequest("Exception: Missing collection_name parameter");
        
        if (isIdRequired)
        {
            if (id == null || id.isEmpty())
                return badRequest("Exception: Missing id parameter");
            else if (!ObjectId.isValid(id))
                return badRequest("Exception: Invalid id");
        }

        if (isDataRequired && data == null)
            return badRequest("Exception: Missing body");
        return null;
    }

    public CompletableFuture<Void> ping()
    {
       return __crud_client.ping();
    }

    public CompletableFuture<REST_Response> getAsync(String dbName, String collectionName, String id)
    {
        REST_Response exception = verifyParameters(dbName, collectionName, id, true, null, false);
        if (exception != null)
            return CompletableFuture.supplyAsync(() -> exception);

        ObjectId objectId = new ObjectId(id);
        return __crud_client.readAsync(dbName, collectionName, objectId)
            .thenApply(document -> {
                if (document != null)
                    return ok(document);
                return notFound(objectId);
            })
            .exceptionally(e-> errorMessage(e));
    }

    public CompletableFuture<REST_Response> postAsync(String dbName, String collectionName, String data)
    {
        REST_Response exception = verifyParameters(dbName, collectionName, null, false, data, true);
        if (exception != null)
            return CompletableFuture.supplyAsync(() -> exception);

        return __crud_client.createAsync(dbName, collectionName, BsonDocument.parse(data))
            .thenApply(documentId -> {
                if (documentId != null)
                    return created(documentId);
                return errorMessage("Failed to create document");
            })
            .exceptionally(e-> errorMessage(e));
    }

    public CompletableFuture<REST_Response> putAsync(String dbName, String collectionName, String id, String data)
    {
        REST_Response exception = verifyParameters(dbName, collectionName, id, true, data, true);
        if (exception != null)
            return CompletableFuture.supplyAsync(() -> exception);

        ObjectId objectId = new ObjectId(id);
        return __crud_client.updateAsync(dbName, collectionName, objectId, BsonDocument.parse(data))
            .thenApply(document -> {
                if (document != null)
                    return ok(document);
                return notFound(objectId);
            })
            .exceptionally(e-> errorMessage(e));
    }

    public CompletableFuture<REST_Response> deleteAsync(String dbName, String collectionName, String id)
    {
        REST_Response exception = verifyParameters(dbName, collectionName, id, true, null, false);
        if (exception != null)
            return CompletableFuture.supplyAsync(() -> exception);
        
        ObjectId objectId = new ObjectId(id);
        return __crud_client.deleteAsync(dbName, collectionName, objectId)
            .thenApply(result -> {
                if (result)
                    return deleted(id);
                return notFound(objectId);
            })
            .exceptionally(e-> errorMessage(e));
    }
}
