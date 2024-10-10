using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using Kafka;

namespace Kafka.CloudEvents.Tests;

public class CloudEventTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        string specVersion = "1.0";
        string type = "com.example.someevent";
        string source = "/mycontext";
        string subject = "123";
        string dataContentType = "application/json";
        var data = new { key = "value" };

        // Act
        var cloudEvent = new CloudEvent(specVersion, type, source, subject, dataContentType, data);

        // Assert
        Assert.Equal(specVersion, cloudEvent.SpecVersion);
        Assert.Equal(type, cloudEvent.Type);
        Assert.Equal(source, cloudEvent.Source);
        Assert.Equal(subject, cloudEvent.Subject);
        Assert.Matches(@"^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$", cloudEvent.Id);
        Assert.True((DateTime.UtcNow - cloudEvent.Time)?.TotalSeconds < 1);
        Assert.Equal(dataContentType, cloudEvent.DataContentType);
        Assert.Equal(data, cloudEvent.Data);
    }

    [Fact]
    public void ToDict_ShouldReturnCorrectDictionaryRepresentation()
    {
        // Arrange
        string specVersion = "1.0";
        string type = "com.example.someevent";
        string source = "/mycontext";
        string subject = "123";
        string dataContentType = "application/json";
        var data = new { key = "value" };
        var cloudEvent = new CloudEvent(specVersion, type, source, subject, dataContentType, data);

        // Act
        var dict = cloudEvent.ToDict();

        // Assert
        Assert.Equal(specVersion, dict["specversion"]);
        Assert.Equal(type, dict["type"]);
        Assert.Equal(source, dict["source"]);
        Assert.Equal(subject, dict["subject"]);
        Assert.Equal(cloudEvent.Id, dict["id"]);
        Assert.Equal(cloudEvent.Time?.ToString("o"), dict["time"]);
        Assert.Equal(dataContentType, dict["datacontenttype"]);
        Assert.Equal(data, dict["data"]);
    }

    [Fact]
    public void FromJson_ShouldReturnCorrectEventRepresentation()
    {
        // Arrange
        var json = @"{
            ""specversion"": ""1.0"",
            ""type"": ""com.example.someevent"",
            ""source"": ""/mycontext"",
            ""subject"": ""123"",
            ""id"": ""a1b2c3"",
            ""time"": ""2021-09-15T10:00:00Z"",
            ""datacontenttype"": ""application/json"",
            ""data"": { ""key"": ""value"" }
        }";

        // Act
        var cloudEvent = CloudEvent.FromJson(json);

        // Assert
        Assert.NotNull(cloudEvent);
        Assert.Equal("1.0", cloudEvent.SpecVersion);
        Assert.Equal("com.example.someevent", cloudEvent.Type);
        Assert.Equal("/mycontext", cloudEvent.Source);
        Assert.Equal("123", cloudEvent.Subject);
        Assert.Equal("a1b2c3", cloudEvent.Id);
        Assert.Equal(new DateTime(2021, 9, 15, 10, 0, 0, DateTimeKind.Utc), cloudEvent.Time?.ToUniversalTime());
        Assert.Equal("application/json", cloudEvent.DataContentType);
        Assert.Equal(@"{""key"":""value""}", JsonSerializer.Serialize(cloudEvent.Data));
    }
}