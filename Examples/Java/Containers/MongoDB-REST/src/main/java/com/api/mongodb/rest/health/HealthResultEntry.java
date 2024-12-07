package com.api.mongodb.rest.health;

public class HealthResultEntry {
    private String key;
    private boolean healthy;
    private String duration;
    private String status;
    private String error;

    public HealthResultEntry(String key, boolean healthy, long duration, String error) {
        this.key = key;
        this.duration = duration + " ms";
        this.healthy = healthy;
        this.status = healthy ? "healthy" : "unhealthy";
        this.error = error;
    }

    public String getKey() {
        return key;
    }

    public boolean isHealthy() {
        return healthy;
    }

    public String getDuration() {
        return duration;
    }

    public String getStatus() {
        return status;
    }

    public String getError() {
        return error;
    }
}