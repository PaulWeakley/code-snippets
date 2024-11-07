using MongoDB.Bson;
using MongoDB.CRUD.Client;

namespace MongoDB.REST.Client;

public class MongoDB_REST_Client
{
    private readonly IMongoDB_CRUD_Client __crud_client;

    public MongoDB_REST_Client(IMongoDB_CRUD_Client mongodb_crud_client)
    {
        __crud_client = mongodb_crud_client;
    }

    static private REST_Response BadRequest(string message)
    {
        return new REST_Response(400, "text/plain", message);
    }

    static private REST_Response ErrorMessage(Exception e)
    {
        return new REST_Response(500, "text/plain", e.Message);
    }

    static private REST_Response NotFound(ObjectId id)
    {
        return new REST_Response(404, "text/plain", $"Exception: Document with id {id} not found");
    }

    static private REST_Response Ok(BsonDocument data)
    {
        return new REST_Response(200, "application/json", data.ToJson());
    }

    static private REST_Response? VerifyParameters(string? dbName, string? collectionName, string? id, bool isIdRequired, object? data, bool isDataRequired)
    {
        if (string.IsNullOrEmpty(dbName))
            return BadRequest("Exception: Missing db_name parameter");
        
        if (string.IsNullOrEmpty(collectionName))
            return BadRequest("Exception: Missing collection_name parameter");
        
        if (isIdRequired)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Exception: Missing id parameter");
            else if (!ObjectId.TryParse(id, out _))
                return BadRequest("Exception: Invalid id");
        }

        if (isDataRequired && data == null)
            return BadRequest("Exception: Missing body");
        return null;
    }

    public async Task Ping()
    {
       await __crud_client.Ping();
    }

    public async Task<REST_Response> GetAsync(string? dbName, string? collectionName, string? id)
    {
        try
        {
            var Exception = VerifyParameters(dbName, collectionName, id, true, null, false);
            if (Exception != null)
            {
                return Exception;
            }
            var objectId = new ObjectId(id);
            var document = await __crud_client.ReadAsync(dbName!, collectionName!, objectId);
            if (document != null)
            {
                return Ok(document);
            }
            return NotFound(objectId);
        }
        catch (Exception e)
        {
            return ErrorMessage(e);
        }
    }

    public async Task<REST_Response> PostAsync(string? dbName, string? collectionName, string? data)
    {
        try
        {
            var Exception = VerifyParameters(dbName, collectionName, null, false, data, true);
            if (Exception != null)
            {
                return Exception;
            }

            var documentId = await __crud_client.CreateAsync(dbName!, collectionName!, BsonDocument.Parse(data));
#pragma warning disable CS8604 // Possible null reference argument.
            return null != documentId && documentId != ObjectId.Empty
                ? new REST_Response(201, "text/plain", documentId.ToString())
                : new REST_Response(500, "text/plain", "Failed to create document");
#pragma warning restore CS8604 // Possible null reference argument.
        }
        catch (Exception e)
        {
            return ErrorMessage(e);
        }
    }

    public async Task<REST_Response> PutAsync(string? dbName, string? collectionName, string? id, string? data)
    {
        try
        {
            var Exception = VerifyParameters(dbName, collectionName, id, true, data, true);
            if (Exception != null)
            {
                return Exception;
            }
            var objectId = new ObjectId(id);
            var document = await __crud_client.UpdateAsync(dbName!, collectionName!, objectId, BsonDocument.Parse(data));
            if (document != null)
            {
                return Ok(document);
            }
            return NotFound(objectId);
        }
        catch (Exception e)
        {
            return ErrorMessage(e);
        }
    }

    public async Task<REST_Response> DeleteAsync(string? dbName, string? collectionName, string? id)
    {
        try
        {
            var Exception = VerifyParameters(dbName, collectionName, id, true, null, false);
            if (Exception != null)
            {
                return Exception;
            }
            var objectId = new ObjectId(id);
            var result = await __crud_client.DeleteAsync(dbName!, collectionName!, objectId);
            if (result)
            {
                return new REST_Response(200, "text/plain", $"Document with id {id} deleted");
            }
            return NotFound(objectId);
        }
        catch (Exception e)
        {
            return ErrorMessage(e);
        }
    }
}
