use serde::{Serialize, Deserialize};

#[derive(Serialize, Deserialize)]
pub struct HealthResultEntry {
    pub key: String,
    pub healthy: bool,
    pub duration: String,
    pub status: String,
    pub error: Option<String>,
}

impl HealthResultEntry {
    pub fn new(key: String, healthy: bool, duration: u64, error: Option<String>) -> Self {
        let status = if healthy { "healthy".to_string() } else { "unhealthy".to_string() };
        let duration = format!("{} ms", duration);
        HealthResultEntry {
            key,
            healthy,
            duration,
            status,
            error,
        }
    }
}
