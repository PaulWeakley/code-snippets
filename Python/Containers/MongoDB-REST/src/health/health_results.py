import time
from typing import List

from .health_result_entry import HealthResultEntry

class HealthResults:
    def __init__(self, duration: int, entries: List[HealthResultEntry]):
        status = 'healthy'
        healthy = True
        for entry in entries:
            if not entry.healthy:
                status = 'unhealthy'
                healthy = False
                break

        self.duration = f'{duration} ms'
        self.healthy = healthy
        self.status = status
        self.results = entries
        self.timestamp = time.strftime('%Y-%m-%dT%H:%M:%SZ', time.gmtime())
        self.version = '1.0.0'

    def to_dict(self):
        return {
            "duration": self.duration,
            "healthy": self.healthy,
            "status": self.status,
            "results": [entry.to_dict() for entry in self.results],
            "timestamp": self.timestamp,
            "version": self.version
        }