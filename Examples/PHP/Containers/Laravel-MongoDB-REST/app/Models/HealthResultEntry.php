<?php

namespace App\Models;

class HealthResultEntry implements \JsonSerializable
{
    private string $key;
    private string $duration;
    private bool $healthy;
    private string $status;
    private ?string $error;

    public function __construct(string $key, int $duration, string $status, ?string $error = null)
    {
        $this->key = $key;
        $this->duration = $duration . " ms";
        $this->status = strtolower($status);
        $this->healthy = $this->status == 'healthy';
        $this->error = $error;
    }

    public function getKey(): string
    {
        return $this->key;
    }

    public function getDuration(): string
    {
        return $this->duration;
    }

    public function getStatus(): string
    {
        return $this->status;
    }

    public function getHealthy(): bool
    {
        return $this->healthy;
    }

    public function getError(): ?string
    {
        return $this->error;
    }

    public function jsonSerialize(): array
    {
        return [
            'key' => $this->key,
            'duration' => $this->duration,
            'healthy' => $this->healthy,
            'status' => $this->status,
            'error' => $this->error,
        ];
    }
}