import { HealthResultEntry } from './health-result-entry.model';

export class HealthResults {
    type: string;
    duration: string;
    healthy: boolean;
    status: string;
    results: HealthResultEntry[];
    timestamp: string;
    version: string;

    constructor(duration: number, entries: HealthResultEntry[]) {
        let status = 'healthy';
        let healthy = true;
        for (const entry of entries) {
            if (!entry.healthy) {
                status = 'unhealthy';
                healthy = false;
                break;
            }
        }
        this.type = 'nodejs-typescript';
        this.duration = `${duration} ms`;
        this.healthy = healthy;
        this.status = status;
        this.results = entries;
        this.timestamp = new Date().toISOString();
        this.version = '1.0.0';
    }

    toDict() {
        return {
            type: this.type,
            duration: this.duration,
            healthy: this.healthy,
            status: this.status,
            results: this.results.map(entry => entry.toDict()),
            timestamp: this.timestamp,
            version: this.version
        };
    }
}