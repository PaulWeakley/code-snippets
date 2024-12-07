using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MongoDB.REST.Health;

public class HealthResultEntry(string key, TimeSpan duration, HealthStatus status, string? error)
{
    public string Key { get; } = key;
    public string Duration { get; } = $"{duration.Milliseconds} ms";
    public bool Healthy { get; } = status == HealthStatus.Healthy;
    public string Status { get; } = status.ToString().ToLower();
    public string? Error { get; } = error;
}