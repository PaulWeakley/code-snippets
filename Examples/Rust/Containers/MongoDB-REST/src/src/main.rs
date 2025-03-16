use dotenv::dotenv;
use env_logger;
use std::env;
use actix_web::{web, App, HttpServer};

mod health;
mod services;
mod controllers;

use crate::services::mongodb::crud::mongodb_crud_client::MongoDBCRUDClient;
use crate::services::mongodb::crud::mongodb_config::MongoDBConfig;
use crate::services::mongodb::rest::mongodb_rest_client::MongoDBRESTClient;

use crate::controllers::health_controller;
use crate::controllers::mongodb_rest_controller;
use crate::controllers::telemetry_controller;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    // Load environment variables from .env file
    dotenv().ok(); 

    // Set logging level before initializing env_logger
    env::set_var("RUST_LOG", "actix_web=info,actix_server=info");
    env::set_var("RUST_BACKTRACE", "1");
    env_logger::init(); // Initialize logging

    // Load MongoDB configuration safely
    let mongodb_config = MongoDBConfig {
        server: env::var("MONGODB_SERVER").unwrap_or_else(|_| "mongodb://localhost:27017".to_string()),
        username: env::var("MONGODB_USERNAME").unwrap_or_else(|_| "".to_string()),
        password: env::var("MONGODB_PASSWORD").unwrap_or_else(|_| "".to_string()),
        app_name: env::var("MONGODB_APP_NAME").unwrap_or_else(|_| "MyApp".to_string()),
        retry_writes: env::var("MONGODB_RETRY_WRITES").unwrap_or_else(|_| "true".to_string()).parse().unwrap_or(true),
        write_concern: env::var("MONGODB_WRITE_CONCERN").unwrap_or_else(|_| "majority".to_string()),
        min_pool_size: env::var("MONGODB_MIN_POOL_SIZE").unwrap_or_else(|_| "1".to_string()).parse().unwrap_or(1),
        max_pool_size: env::var("MONGODB_MAX_POOL_SIZE").unwrap_or_else(|_| "10".to_string()).parse().unwrap_or(10),
        wait_queue_timeout_ms: env::var("MONGODB_WAIT_QUEUE_TIMEOUT_MS").unwrap_or_else(|_| "1000".to_string()).parse().unwrap_or(1000),
    };

    // Initialize MongoDB clients
    let mongodb_crud_client = MongoDBCRUDClient::new(mongodb_config).await;
    let mongodb_rest_client = MongoDBRESTClient::new(mongodb_crud_client);
    let app_state = web::Data::new(mongodb_rest_client);

    // Read port from environment
    let port: u16 = env::var("HOST_PORT")
        .unwrap_or_else(|_| "8080".to_string())
        .parse()
        .unwrap_or(8080);

    // Start Actix Web server
    HttpServer::new(move || {
        App::new()
            .app_data(app_state.clone())
            .configure(health_controller::configure)
            .configure(mongodb_rest_controller::configure)
            .configure(telemetry_controller::configure)
    })
    .bind(("0.0.0.0", port))?
    .run()
    .await
}