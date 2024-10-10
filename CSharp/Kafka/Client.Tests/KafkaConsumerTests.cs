using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.Extensions.Configuration;

namespace Kafka.Client.Tests;

public class KafkaConsumerTests
{
    [Fact]
    public async void IntegrationTest_ReceivesMessages()
    {
        var KAFKA_BOOTSTRAP_SERVERS = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
        var KAFKA_CONSUMER_SASL_USERNAME = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_SASL_USERNAME");
        var KAFKA_CONSUMER_SASL_PASSWORD = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_SASL_PASSWORD");

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var kafkaSection = config.GetSection("Kafka");
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = KAFKA_BOOTSTRAP_SERVERS,
            GroupId = kafkaSection.GetValue<string>("group.id"),
            SaslUsername = KAFKA_CONSUMER_SASL_USERNAME,
            SaslPassword = KAFKA_CONSUMER_SASL_PASSWORD,
            SecurityProtocol = kafkaSection.GetValue<SecurityProtocol>("security.protocol"),
            SaslMechanism = kafkaSection.GetValue<SaslMechanism>("sasl.mechanisms"),
            AutoOffsetReset = kafkaSection.GetValue<AutoOffsetReset>("auto.offset.reset"),  // Start reading from earliest message
            EnableAutoCommit = kafkaSection.GetValue<bool>("enable.auto.commit")       // Disable automatic offset committing
        };

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var consumer = new KafkaConsumer(consumerConfig, loggerFactory.CreateLogger<KafkaConsumer>());

        var handlerMock = new Mock<IKafkaEventHandler<string, string>>();
        handlerMock.Setup(h => h.Handle(It.IsAny<Message<string, string>>())).Returns(true);

        var task = consumer.StartConsumingAsync("topic_0", TimeSpan.FromSeconds(5), handlerMock.Object);
        Thread.Sleep(15000);
        consumer.StopConsuming();
        await task;

        handlerMock.Verify(h => h.Handle(It.IsAny<Message<string, string>>()), Times.AtLeastOnce);
    }
}
