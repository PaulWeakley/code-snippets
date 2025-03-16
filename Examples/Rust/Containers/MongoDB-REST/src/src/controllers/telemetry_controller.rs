use actix_web::{post, web, HttpResponse, Responder};
use serde_json::json;
use serde_json::Value;
use mongodb::{bson::{ doc }};
use serde::{Deserialize};

use chrono::{Utc, TimeZone};
use std::time::{SystemTime, UNIX_EPOCH};

use crate::services::mongodb::rest::mongodb_rest_client::MongoDBRESTClient;

#[derive(Deserialize)]
struct PathParams {
    raw_start: f64
}

/// Registers routes in Actix-web
pub fn configure(cfg: &mut web::ServiceConfig) {
    cfg.service(
        web::scope("/api/telemetry")
            .service(post)
        );
}

#[post("/{raw_start}")]
async fn post(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>, body: web::Bytes) -> impl Responder {
    let params = path.into_inner();
    let client = mongodb_rest_client.get_client();
    
    let raw_start = params.raw_start;
    let seconds = raw_start as i64;
    let nanos = ((raw_start % 1.0) * 1_000_000_000.0) as u32;
    // Use timestamp_opt instead of timestamp
    let start = Utc.timestamp_opt(seconds, nanos).single().expect("Invalid timestamp");
    let container_time = SystemTime::now().duration_since(UNIX_EPOCH).unwrap().as_secs_f64();
    let ssl_handshake = Utc::now().signed_duration_since(start);
    let start_ref = Utc::now();

    let measure = Utc::now();
    let received_data = String::from_utf8_lossy(&body);
    let body: Option<Value> = serde_json::from_str(&received_data).ok().unwrap();
    let deserialization = Utc::now().signed_duration_since(measure);

    let db_name = "test";
    let collection_name = "source";
    let measure = Utc::now();
    let document_id = client.create(db_name, collection_name, body).await.unwrap();
    let create_object = Utc::now().signed_duration_since(measure);

    let measure = Utc::now();
    let document = client.read(db_name, collection_name, document_id).await.unwrap();
    let get_object = Utc::now().signed_duration_since(measure);

    let measure = Utc::now();
    let _json_data = json!(document);
    let serialization = Utc::now().signed_duration_since(measure);

    let measure = Utc::now();
    for _ in 0..15 {
        client.update(db_name, collection_name, document_id, doc! {"time": SystemTime::now().duration_since(UNIX_EPOCH).unwrap().as_secs_f64()}).await.unwrap();
    }
    let high_iops = Utc::now().signed_duration_since(measure);

    let measure = Utc::now();
    client.delete(db_name, collection_name, document_id).await.unwrap();
    let delete_object = Utc::now().signed_duration_since(measure);

    let total = Utc::now().signed_duration_since(start_ref);

    let telemetry = doc! {
        "start": raw_start,
        "container_time": container_time,
        "ssl_handshake": ssl_handshake.num_milliseconds(),
        "deserialization": deserialization.num_milliseconds(),
        "create_object": create_object.num_milliseconds(),
        "get_object": get_object.num_milliseconds(),
        "serialization": serialization.num_milliseconds(),
        "high_iops": high_iops.num_milliseconds(),
        "delete_object": delete_object.num_milliseconds(),
        "total": total.num_milliseconds(),
        "type": "Rust"
    };

    // Return telemetry data as JSON
    HttpResponse::Ok().json(telemetry)
}