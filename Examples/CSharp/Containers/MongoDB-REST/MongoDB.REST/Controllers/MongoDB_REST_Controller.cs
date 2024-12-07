using Microsoft.AspNetCore.Mvc;
using MongoDB.REST.Client;
using MongoDB.CRUD.Client;
using System.Text.Json;

namespace MongoDB.REST.Controllers;

[ApiController]
[Route("api/mongodb")]
public class MongoDB_REST_Controller : ControllerBase
{
    static private IActionResult ToActionResult(REST_Response response)
    {
        return new ContentResult
        {
            StatusCode = response.StatusCode,
            ContentType = response.ContentType,
            Content = response.Body
        };
    }

    private readonly IMongoDB_Client_Builder _mongoDB_Client_Builder;

    public MongoDB_REST_Controller(IMongoDB_Client_Builder mongoDB_Client_Builder)
    {
        _mongoDB_Client_Builder = mongoDB_Client_Builder;
    }

    private MongoDB_REST_Client CreateMongoDBClient()
    {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client(_mongoDB_Client_Builder));
    }

    [HttpGet("{db_name}/{collection_name}/{id}")]
    public async Task<IActionResult> Get(string db_name, string collection_name, string id)
    {
        return ToActionResult(await CreateMongoDBClient().GetAsync(db_name, collection_name, id));
    }

    [HttpPost("{db_name}/{collection_name}")]
    public async Task<IActionResult> Post(string db_name, string collection_name, [FromBody] object body)
    {
        return ToActionResult(await CreateMongoDBClient().PostAsync(db_name, collection_name, JsonSerializer.Serialize(body)));
    }

    [HttpPut("{db_name}/{collection_name}/{id}")]
    [HttpPatch("{db_name}/{collection_name}/{id}")]
    public async Task<IActionResult> Put(string db_name, string collection_name, string id, [FromBody] object body)
    {
        return ToActionResult(await CreateMongoDBClient().PutAsync(db_name, collection_name, id, JsonSerializer.Serialize(body)));
    }

    [HttpDelete("{db_name}/{collection_name}/{id}")]
    public async Task<IActionResult> Delete(string db_name, string collection_name, string id)
    {
        return ToActionResult(await CreateMongoDBClient().DeleteAsync(db_name, collection_name, id));
    }
}