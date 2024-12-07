using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka.Client;

public class KafkaProducer(ProducerConfig config, ILogger? logger = default)
{
    private ILogger? Logger { get; } = logger;
    private ProducerConfig Config { get; } = config;

    public async Task<bool> ProduceMessagesAsync<TDataType, TKafkaKey, TKafkaEvent>(string topic, 
                                IKafkaEventBuilder<TDataType, TKafkaKey, TKafkaEvent> eventBuilder, 
                                IEnumerable<TDataType> messages,
                                Dictionary<string, string>? headers = null,
                                Action<DeliveryReport<TKafkaKey, TKafkaEvent>>? callback = null, 
                                CancellationToken cancellationToken = default)
    {
        var producer = new ProducerBuilder<TKafkaKey, TKafkaEvent>(Config).Build();
        foreach (var message in messages)
        {
            var kafkaEvent = eventBuilder.BuildEvent(message, headers);
            try
            {
                await producer.ProduceAsync(topic, kafkaEvent, cancellationToken);
                Logger?.LogInformation($"Produced message: {kafkaEvent.Value}");
            }
            catch (ProduceException<TKafkaKey, TKafkaEvent> e)
            {
                Logger?.LogError($"Delivery failed: {e.Error.Reason}");
            }
            catch (KafkaException)
            {
                Logger?.LogError("Local producer queue is full. Waiting to send message...");
                producer.Flush(TimeSpan.FromSeconds(10)); // Wait for messages to be delivered
                producer.Produce(topic, kafkaEvent, callback);
            }
        }
        producer.Flush(TimeSpan.FromSeconds(10));
        producer.Dispose();
        return true;
    }
}