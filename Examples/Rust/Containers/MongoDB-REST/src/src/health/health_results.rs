use serde::{Serialize, Deserialize};

use crate::health::health_result_entry::HealthResultEntry;

#[derive(Serialize, Deserialize)]
pub struct HealthResults {
    pub r#type: String,
    pub duration: String,
    pub healthy: bool,
    pub status: String,
    pub results: Vec<HealthResultEntry>,
    pub timestamp: String,
    pub version: String,
}

impl HealthResults {
    pub fn new(duration: u64, entries: Vec<HealthResultEntry>) -> Self {
        let mut status = "healthy".to_string();
        let mut healthy = true;
        for entry in &entries {
            if !entry.healthy {
                status = "unhealthy".to_string();
                healthy = false;
                break;
            }
        }
        HealthResults {
            r#type: "rust".to_string(),
            duration: format!("{} ms", duration),
            healthy,
            status,
            results: entries,
            timestamp: chrono::Utc::now().to_rfc3339(),
            version: "1.0.0".to_string(),
        }
    }
}