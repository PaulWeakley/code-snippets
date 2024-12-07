using Confluent.Kafka;
using Kafka.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Azure.Function.Kafka.REST
{
    public class HttpPost
    {
        private readonly ILogger<HttpPost> _logger;

        public HttpPost(ILogger<HttpPost> logger)
        {
            _logger = logger;
        }

        [Function("PublishEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{topic}/{subject}")] HttpRequest request)
        {
            _logger.LogInformation("Received request: " + request.Body);

            var topic = string.Empty;
            var subject = string.Empty;
            var data = string.Empty;
            var useInterop = false;

            if (request.RouteValues != null)
            {
                if (request.RouteValues.TryGetValue("topic", out var topicValue))
                    topic = topicValue?.ToString();
                if (request.RouteValues.TryGetValue("subject", out var subjectValue))
                    subject = subjectValue?.ToString();
            }

            using (var reader = new StreamReader(request.Body))
                data = await reader.ReadToEndAsync();

            if (request.Query != null)
            {
                request.Query.TryGetValue("interop", out var interop_values);
                var interop_value = interop_values.FirstOrDefault();
                if (!string.IsNullOrEmpty(interop_value))
                    useInterop = "true".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "1".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "yes".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase) ||
                                "on".Equals(interop_value, StringComparison.CurrentCultureIgnoreCase);
            }

            if (string.IsNullOrEmpty(topic))
                return new BadRequestObjectResult("Error: Missing topic parameter");

            if (string.IsNullOrEmpty(data))
                return new BadRequestObjectResult("Error: Missing body");

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
                     messages: [data],
                     headers: new CloudEventHeaders(subject: subject));

            // Return the API Gateway response
            return result ? new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent }
                        : new ObjectResult("Error: Failed to produce message") { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
