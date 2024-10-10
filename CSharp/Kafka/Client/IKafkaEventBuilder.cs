using Confluent.Kafka;

namespace Kafka.Client;

public interface IKafkaEventBuilder<TDataType, TKafkaKey, TKafkaEvent>
{
    Message<TKafkaKey, TKafkaEvent> BuildEvent(TDataType data, Dictionary<string, string>? headers = null);
}