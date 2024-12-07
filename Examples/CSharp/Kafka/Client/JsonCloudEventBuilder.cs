using System.Text.Json;
using Confluent.Kafka;

namespace Kafka.Client;
public class JsonCloudEventBuilder<TDataType>(JsonSerializerOptions? jsonSerializerOptions = null) : CloudEventBuilder<TDataType, string, string>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();

    public override Message<string, string> BuildEvent(TDataType data, Dictionary<string, string>? headers = null)
    {
        CloudEventHeaders.ExtractDefaultHeaders(headers, out var specversion, out var type, out var source, out var subject);

        var cloudEvent = ToCloudEvent(specversion ?? "", type ?? "", source ?? "", subject ?? "", data);

        return new Message<string, string> { Key = cloudEvent!.Id!, Value = JsonSerializer.Serialize(cloudEvent, _jsonSerializerOptions) };
    }

    protected override string GetDataContentType() => "application/json";
}