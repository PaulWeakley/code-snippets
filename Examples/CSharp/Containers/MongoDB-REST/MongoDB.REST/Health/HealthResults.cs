using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MongoDB.REST.Health;

public class HealthResults(HealthReport healthReport)
{
    public string Type { get; } = "csharp";
    public string Duration { get; } = $"{healthReport.TotalDuration.Milliseconds} ms";
    public bool Healthy { get; } = healthReport.Status == HealthStatus.Healthy;
    public string Status { get; } =  healthReport.Status.ToString().ToLower();
    public IEnumerable<HealthResultEntry> Results { get; } = healthReport.Entries.Select(e => 
        new HealthResultEntry(e.Key, e.Value.Duration, e.Value.Status, e.Value.Exception?.Message ?? null));
    public string Timestamp { get; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public string Version { get; } = "1.0.0";
}