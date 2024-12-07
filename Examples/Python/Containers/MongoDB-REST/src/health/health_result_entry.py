class HealthResultEntry:
    def __init__(self, key: str, healthy: bool, duration: int, error: str = None):
        self.key = key
        self.duration = f'{duration} ms'
        self.healthy = healthy
        self.status = 'healthy' if healthy else 'unhealthy'
        self.error = error

    def to_dict(self):
        return {
            "key": self.key,
            "duration": self.duration,
            "healthy": self.healthy,
            "status": self.status,
            "error": self.error
        }