pub struct MongoDBConfig {
    pub username: String,
    pub password: String,
    pub server: String,
    pub retry_writes: bool,
    pub write_concern: String,
    pub app_name: String,
    pub min_pool_size: u32,
    pub max_pool_size: u32,
    pub wait_queue_timeout_ms: u64,
}

impl MongoDBConfig {
    pub fn to_uri(&self) -> String {
        format!(
            "mongodb+srv://{}:{}@{}/?retryWrites={}&w={}&appName={}&minPoolSize={}&maxPoolSize={}&waitQueueTimeoutMS={}",
            self.username,
            self.password,
            self.server,
            self.retry_writes,
            self.write_concern,
            self.app_name,
            self.min_pool_size,
            self.max_pool_size,
            self.wait_queue_timeout_ms
        )
    }
}