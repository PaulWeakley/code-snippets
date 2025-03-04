use actix_web::{web, App, HttpServer};

mod health;
mod services;
mod controllers;

use crate::services::mongodb::crud::mongodb_crud_client::MongoDBCRUDClient;
use crate::services::mongodb::crud::mongodb_config::MongoDBConfig;
use crate::services::mongodb::rest::mongodb_rest_client::MongoDBRESTClient;

use crate::controllers::health_controller;
use crate::controllers::mongodb_rest_controller;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    let mongodb_config = MongoDBConfig {
        server: "development.tovxj.mongodb.net".to_string(),
        username: "paulweakley".to_string(),
        password: "p8aYxUCcw04Jt7TE".to_string(),
        app_name: "Cluster0".to_string(),
        retry_writes: true,
        write_concern: "majority".to_string(),
        min_pool_size: 10,
        max_pool_size: 50,
        wait_queue_timeout_ms: 1000,
    };
    let mongodb_crud_client = MongoDBCRUDClient::new(mongodb_config).await;
    let mongodb_rest_client = MongoDBRESTClient::new(mongodb_crud_client);
    let app_state = web::Data::new(mongodb_rest_client);
    // Bind to 0.0.0.0 so the container can expose the port
    HttpServer::new(move || App::new()
        .app_data(app_state.clone())
        .configure(health_controller::configure)
        .configure(mongodb_rest_controller::configure))
        .bind("0.0.0.0:8080")?
        .run()
        .await
}
