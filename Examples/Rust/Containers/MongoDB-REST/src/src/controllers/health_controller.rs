use actix_web::{web, HttpResponse, Responder};
use std::time::Instant;
use tokio::join;

use crate::services::mongodb::rest::mongodb_rest_client::MongoDBRESTClient;
use crate::health::health_results::HealthResults;
use crate::health::health_result_entry::HealthResultEntry;

/// Asynchronously checks MongoDB health.
async fn build_health_check_mongodb(mongodb_rest_client: web::Data<MongoDBRESTClient>) -> HealthResultEntry {
    let start = Instant::now();
    match mongodb_rest_client.ping().await {
        Ok(..) => HealthResultEntry::new("mongodb".to_string(), true, start.elapsed().as_millis() as u64, None),
        Err(e) => HealthResultEntry::new("mongodb".to_string(), false, start.elapsed().as_millis() as u64, Some(e.to_string())),
    }
}

/// Builds the full health check response, running checks in parallel.
async fn build_health_check_response(mongodb_rest_client: web::Data<MongoDBRESTClient>) -> HealthResults {
    let start = Instant::now();
    // Run checks concurrently using `join!`
    let (mongodb_result,) = join!(build_health_check_mongodb(mongodb_rest_client));
    HealthResults::new(start.elapsed().as_millis() as u64, vec![mongodb_result])
}

/// Health check endpoint
async fn health_check(mongodb_rest_client: web::Data<MongoDBRESTClient>) -> impl Responder {
    let health_results = build_health_check_response(mongodb_rest_client).await;
    if health_results.healthy {
        HttpResponse::Ok().json(health_results)
    } else {
        HttpResponse::InternalServerError().json(health_results)
    }
}

/// Registers routes in Actix-web
pub fn configure(cfg: &mut web::ServiceConfig) {
    cfg.service(
        web::scope("/api/health")
            .route("", web::get().to(health_check))
            .route("", web::post().to(health_check))
        );
}
