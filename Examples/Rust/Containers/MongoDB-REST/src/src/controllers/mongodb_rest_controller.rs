use actix_web::{get, post, put, patch, delete, web, HttpResponse, Responder, http::StatusCode};
use serde::Deserialize;
use serde_json::Value;

use crate::services::mongodb::rest::mongodb_rest_client::MongoDBRESTClient;

#[derive(Deserialize)]
struct PathParams {
    db_name: String,
    collection_name: String,
    id: Option<String>,
}

/// Registers routes in Actix-web
pub fn configure(cfg: &mut web::ServiceConfig) {
    cfg.service(
        web::scope("/api/mongodb")
            .service(get)
            .service(post)
            .service(put)
            .service(patch)
            .service(delete)
        );
}

#[get("/{db_name}/{collection_name}/{id}")]
async fn get(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>) -> impl Responder {
    let params = path.into_inner();
    let response = mongodb_rest_client.get(Some(&params.db_name), Some(&params.collection_name), params.id.as_deref()).await;
    HttpResponse::build(StatusCode::from_u16(response.status_code).unwrap_or(StatusCode::INTERNAL_SERVER_ERROR))
        .content_type(response.content_type)
        .body(response.body)
}

#[post("/{db_name}/{collection_name}")]
async fn post(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>, body: web::Bytes) -> impl Responder {
    let params = path.into_inner();
    let received_data = String::from_utf8_lossy(&body);
    let json_value: Option<Value> = serde_json::from_str(&received_data).ok();
    let response = mongodb_rest_client.post(Some(&params.db_name), Some(&params.collection_name), json_value).await;
    HttpResponse::build(StatusCode::from_u16(response.status_code).unwrap_or(StatusCode::INTERNAL_SERVER_ERROR))
        .content_type(response.content_type)
        .body(response.body)
}

async fn put_patch(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>, body: web::Bytes) -> impl Responder {
    let params = path.into_inner();
    let received_data = String::from_utf8_lossy(&body);
    let json_value: Option<Value> = serde_json::from_str(&received_data).ok();
    let response = mongodb_rest_client.put(Some(&params.db_name), Some(&params.collection_name), params.id.as_deref(), json_value).await;
    HttpResponse::build(StatusCode::from_u16(response.status_code).unwrap_or(StatusCode::INTERNAL_SERVER_ERROR))
        .content_type(response.content_type)
        .body(response.body)
}

#[put("/{db_name}/{collection_name}/{id}")]
async fn put(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>, body: web::Bytes) -> impl Responder {
    put_patch(mongodb_rest_client, path, body).await
}

#[patch("/{db_name}/{collection_name}/{id}")]
async fn patch(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>, body: web::Bytes) -> impl Responder {
    put_patch(mongodb_rest_client, path, body).await
}

#[delete("/{db_name}/{collection_name}/{id}")]
async fn delete(mongodb_rest_client: web::Data<MongoDBRESTClient>, path: web::Path<PathParams>) -> impl Responder {
    let params = path.into_inner();
    let response = mongodb_rest_client.delete(Some(&params.db_name), Some(&params.collection_name), params.id.as_deref()).await;
    HttpResponse::build(StatusCode::from_u16(response.status_code).unwrap_or(StatusCode::INTERNAL_SERVER_ERROR))
        .content_type(response.content_type)
        .body(response.body)
}