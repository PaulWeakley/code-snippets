package com.api.mongodb.rest.health;

import java.util.List;

public class HealthResults {
    private String duration;
    private boolean healthy;
    private String status;
    private List<HealthResultEntry> results;
    private String timestamp;
    private String version;

    public HealthResults(long duration, List<HealthResultEntry> entries) {
        String status = "healthy";
        boolean healthy = true;
        for (HealthResultEntry entry : entries) {
            if (!entry.isHealthy()) {
                status = "unhealthy";
                healthy = false;
                break;
            }
        }

        this.duration = duration + " ms";
        this.healthy = healthy;
        this.status = status;
        this.results = entries;
        this.timestamp = new java.util.Date().toString();
        this.version = "1.0.0";
    }

    public String getDuration() {
        return duration;
    }

    public boolean isHealthy() {
        return healthy;
    }

    public String getStatus() {
        return status;
    }

    public List<HealthResultEntry> getResults() {
        return results;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public String getVersion() {
        return version;
    }
}