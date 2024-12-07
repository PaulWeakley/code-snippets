using Confluent.Kafka;

namespace Kafka.Client;

public interface IKafkaEventHandler<TKafkaKey, TKafkaEvent>
{
    bool Handle(Message<TKafkaKey, TKafkaEvent> message);
}