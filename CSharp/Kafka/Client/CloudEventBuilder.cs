using System.Text.Json;
using Confluent.Kafka;

namespace Kafka.Client;

public abstract class CloudEventBuilder<TDataType, TKafkaKey, TKafkaEvent> : IKafkaEventBuilder<TDataType, TKafkaKey, TKafkaEvent>
{
    public abstract Message<TKafkaKey, TKafkaEvent> BuildEvent(TDataType data, Dictionary<string, string>? headers = null);
    protected abstract string GetDataContentType();

    protected CloudEvent ToCloudEvent(string specVersion, string type, string source, string subject, TDataType data)
    {
        return new CloudEvent
        (
            specVersion: specVersion,
            type: type,
            source: source,
            subject: subject,
            dataContentType: GetDataContentType(),
            data: data
        );
    }


}