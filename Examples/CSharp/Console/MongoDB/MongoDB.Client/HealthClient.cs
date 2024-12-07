using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MongoDB.Client;

public class HealthClient
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;


    public HealthClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _client = new HttpClient();
    }

    public async Task<string> GetHealthAsync()
    {
        var response = await _client.GetAsync($"{_baseUrl}");
        return await response.Content.ReadAsStringAsync();
    }
}