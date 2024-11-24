export class HealthResultEntry {
    key: string;
    healthy: boolean;
    duration: string;
    status: string;
    error?: string;

    constructor(key: string, healthy: boolean, duration: number, error?: string) {
        this.key = key;
        this.duration = `${duration} ms`;
        this.healthy = healthy;
        this.status = healthy ? 'healthy' : 'unhealthy';
        this.error = error;
    }

    toDict() {
        return {
            key: this.key,
            duration: this.duration,
            healthy: this.healthy,
            status: this.status,
            error: this.error
        };
    }
}