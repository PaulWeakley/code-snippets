
namespace MongoDB.REST;


public class Telemetry
{
    [JsonPropertyName("start")]
    public double Start { get; set; }
    [JsonPropertyName("container_time")]
    public double ContainerTime { get; set; }
    [JsonPropertyName("ssl_handshake")]
    public double SslHandshake { get; set; }
    [JsonPropertyName("deserialization")]
    public double Deserialization { get; set; }
    [JsonPropertyName("create_object")]
    public double CreateObject { get; set; }
    [JsonPropertyName("get_object")]
    public double GetObject { get; set; }
    [JsonPropertyName("serialization")]
    public double Serialization { get; set; }
    [JsonPropertyName("total")]
    public double Total { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; } = "CSharp";

    [JsonPropertyName("high_iops")]
    public double HighIOps { get; set; }
    [JsonPropertyName("delete_object")]
    public double deleteObject { get; set; }
}