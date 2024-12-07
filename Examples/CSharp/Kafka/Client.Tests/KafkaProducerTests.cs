using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Kafka.Client.Tests;

public class KafkaProducerTests
{
    [Fact]
    public async void IntegrationTest_ProduceMessages()
    {
        var KAFKA_BOOTSTRAP_SERVERS = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
        var KAFKA_PRODUCER_SASL_USERNAME = Environment.GetEnvironmentVariable("KAFKA_PRODUCER_SASL_USERNAME");
        var KAFKA_PRODUCER_SASL_PASSWORD = Environment.GetEnvironmentVariable("KAFKA_PRODUCER_SASL_PASSWORD");

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var kafkaSection = config.GetSection("Kafka");
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = KAFKA_BOOTSTRAP_SERVERS,
            SaslUsername = KAFKA_PRODUCER_SASL_USERNAME,
            SaslPassword = KAFKA_PRODUCER_SASL_PASSWORD,
            SecurityProtocol = kafkaSection.GetValue<SecurityProtocol>("security.protocol"),
            SaslMechanism = kafkaSection.GetValue<SaslMechanism>("sasl.mechanisms"),
            Acks = kafkaSection.GetValue<Acks>("acks"),
            MessageSendMaxRetries = kafkaSection.GetValue<int>("retries"),
            CompressionType = kafkaSection.GetValue<CompressionType>("compression.type"),
            ClientId = kafkaSection.GetValue<string>("client.id")
        };

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var producer = new KafkaProducer(producerConfig, loggerFactory.CreateLogger<KafkaProducer>());

        var result = await producer.ProduceMessagesAsync("topic_0", new JsonCloudEventBuilder<string>(), ["test"]);
        Assert.True(result);
    }
}

