<?php

namespace App\Models;

class HealthResults implements \JsonSerializable
{
    private string $type;
    private string $duration;
    private bool $healthy;
    private string $status;
    private array $results;
    private string $timestamp;
    private string $version;

    public function __construct(int $duration, array $entries)
    {
        $this->type = "php";
        $this->duration = $duration . " ms";
        $this->healthy = true;
        foreach($entries as $entry) {
            if (!$entry->isHealthy()) {
                $this->healthy = false;
                break;
            }
        }
        $this->status = $this->healthy ? 'healthy' : 'unhealthy';
        $this->results = $entries;
        $this->timestamp = gmdate("Y-m-d\TH:i:s\Z");
        $this->version = "1.0.0";
    }

    public function getType(): string
    {
        return $this->type;
    }

    public function getDuration(): string
    {
        return $this->duration;
    }

    public function isHealthy(): bool
    {
        return $this->healthy;
    }

    public function getStatus(): string
    {
        return $this->status;
    }

    public function getResults(): array
    {
        return $this->results;
    }

    public function getTimestamp(): string
    {
        return $this->timestamp;
    }

    public function getVersion(): string
    {
        return $this->version;
    }

    public function jsonSerialize(): array
    {
        return [
            'type' => $this->type,
            'duration' => $this->duration,
            'healthy' => $this->healthy,
            'status' => $this->status,
            'results' => $this->results,
            'timestamp' => $this->timestamp,
            'version' => $this->version,
        ];
    }
}