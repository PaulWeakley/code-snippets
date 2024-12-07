using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kafka;

public class CloudEvent
{
    [JsonPropertyName("specversion")]
    public string? SpecVersion { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
    [JsonPropertyName("id")]
    public string? Id { get; private set; }
    [JsonPropertyName("time")]
    public DateTime? Time { get; private set; }
    [JsonPropertyName("datacontenttype")]
    public string? DataContentType { get; set; }
    [JsonPropertyName("data")]
    public object? Data { get; set; }

    public CloudEvent(string? specVersion = null, string? type = null, string? source = null, string? subject = null, 
                      string? dataContentType = null, object? data = null)
    {
        SpecVersion = specVersion;
        Type = type;
        Source = source;
        Subject = subject;
        Id = Guid.NewGuid().ToString();
        Time = DateTime.UtcNow;
        DataContentType = dataContentType;
        Data = data;
    }

    public Dictionary<string, object?> ToDict()
    {
        return new Dictionary<string, object?>
        {
            { "specversion", SpecVersion },
            { "type", Type },
            { "source", Source },
            { "subject", Subject },
            { "id", Id },
            { "time", Time?.ToString("o") }, // ISO 8601 format
            { "datacontenttype", DataContentType },
            { "data", Data }
        };
    }

    static public CloudEvent? FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        var cloudEvent = JsonSerializer.Deserialize<CloudEvent>(json);
        if (cloudEvent == null)
            return null;

        var jsonObject = JsonSerializer.Deserialize<JsonElement>(json);
        if (jsonObject.TryGetProperty("id", out var idProperty))
            cloudEvent.Id = idProperty.GetString();
        if (jsonObject.TryGetProperty("time", out var timeProperty))
            if (DateTime.TryParse(timeProperty.GetString(), out var time))
                cloudEvent.Time = time;
            else
                cloudEvent.Time = null;
        return cloudEvent;
    }
}