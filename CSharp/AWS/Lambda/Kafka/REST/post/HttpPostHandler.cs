using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Confluent.Kafka;
using Kafka.Client;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWS.Lambda.Kafka.REST
{
    public class HttpPostHandler
    {
        /// <summary>
        /// Function handler to process HTTP requests via API Gateway.
        /// </summary>
        /// <param name="request">The API Gateway request</param>
        /// <param name="context">The Lambda context</param>
        /// <returns>API Gateway response</returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            // Log the request
            context.Logger.LogLine("Received request: " + request.Body);

            var topic = string.Empty;
            var subject = string.Empty;
            var data = string.Empty;
            var useInterop = false;

            if (request.PathParameters != null)
            {
                request.PathParameters.TryGetValue("topic", out topic);
                request.PathParameters.TryGetValue("subject", out subject);
            }

            if (!string.IsNullOrEmpty(request.Body))
                data = request.Body;

            if (request.QueryStringParameters != null)
            {
                request.QueryStringParameters.TryGetValue("interop", out string? interop_value);
                if (!string.IsNullOrEmpty(interop_value))
                    useInterop = "true".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "1".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "yes".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "on".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase);
            }

            if (string.IsNullOrEmpty(topic))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Error: Missing topic parameter"
                };
            }

            if (string.IsNullOrEmpty(data))
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 400,
                    Body = "Error: Missing body"
                };
            }

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

            var producer = new KafkaProducer(producerConfig, null);

            // TODO: Implement Interop Cloud Event Builder
            var result = await producer.ProduceMessagesAsync(topic, 
                     new JsonCloudEventBuilder<string>(), 
                     [data],
                     headers: new CloudEventHeaders(subject: subject));

            // Return the API Gateway response
            return result ? new APIGatewayProxyResponse { StatusCode = 204 }
                        : new APIGatewayProxyResponse { StatusCode = 500, Body = "Error: Failed to produce message" };
        }
    }
}
