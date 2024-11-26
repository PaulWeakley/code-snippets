using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MongoDB.Client;

public class MongoDB_REST_Client
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public MongoDB_REST_Client(string baseUrl)
    {
        _baseUrl = baseUrl;
        _client = new HttpClient();
    }

    private string GetUrl(string dbName, string collectionName, string? id = null)
    {
        var url = $"{_baseUrl}/{dbName}/{collectionName}";
        if (id != null)
        {
            url += $"/{id}";
        }
        return url;
    }

    public async Task<string> CreateAsync(string dbName, string collectionName, object data)
    {
        var response = await _client.PostAsync(GetUrl(dbName, collectionName), new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ReadAsync(string dbName, string collectionName, string id)
    {
        var response = await _client.GetAsync(GetUrl(dbName, collectionName, id));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> UpdateAsync(string dbName, string collectionName, string id, object data)
    {
        var url = $"{_baseUrl}/{dbName}/{collectionName}/{id}";
        var response = await _client.PutAsync(GetUrl(dbName, collectionName, id), new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string dbName, string collectionName, string id)
    {
        var response = await _client.DeleteAsync(GetUrl(dbName, collectionName, id));
        return await response.Content.ReadAsStringAsync();
    }
}