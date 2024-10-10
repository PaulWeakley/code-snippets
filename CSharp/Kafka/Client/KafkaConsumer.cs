using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Kafka.Client;

public class KafkaConsumer(ConsumerConfig config, ILogger? logger = default)
{
    private ILogger? Logger { get; } = logger;
    private ConsumerConfig Config { get; set; } = config;
    private Thread? ConsumerThread  { get; set; }
    private CancellationTokenSource? CancellationTokenSource { get; set; }

    public Task<bool> StartConsumingAsync<TKafkaKey, TKafkaEvent>(string topic, TimeSpan pollingInterval, 
                            IKafkaEventHandler<TKafkaKey, TKafkaEvent> kafkaEventHanlder,
                            CancellationToken cancellationToken = default)
    {
        if (IsStopping)
            throw new Exception("Consumer is stopping. Please wait for the consumer to stop before starting it again.");
        if (IsRunning)
            throw new Exception("Consumer is already running.");
        Logger?.LogInformation("Consumer thread started.");
        CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return Task.Run(() => ConsumeMessagesAsync(topic, pollingInterval, kafkaEventHanlder, CancellationTokenSource.Token), CancellationTokenSource.Token);
    }

    public bool IsRunning => ConsumerThread?.IsAlive ?? false;

    public bool IsStopping => CancellationTokenSource?.IsCancellationRequested ?? false;

    public void StopConsuming()
    {
        CancellationTokenSource?.Cancel();
        ConsumerThread?.Join();
    }

    private Task<bool> ConsumeMessagesAsync<TKafkaKey, TKafkaEvent>(string topic, TimeSpan pollingInterval, 
                                IKafkaEventHandler<TKafkaKey, TKafkaEvent> kafkaEventHanlder, 
                                CancellationToken cancellationToken = default)
    {
        try
        {
            using var consumer = new ConsumerBuilder<TKafkaKey, TKafkaEvent>(Config).Build();
            consumer.Subscribe(topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(pollingInterval);
                if (consumeResult != null)
                {
                    if (kafkaEventHanlder.Handle(consumeResult.Message))
                        consumer.Commit(consumeResult);
                }
            }
            return Task.FromResult(true);
        }
        catch (ConsumeException e)
        {
            Logger?.LogError(e, "Consume error: {Reason}", e.Error.Reason);
        }
        return Task.FromResult(false);
    }
}