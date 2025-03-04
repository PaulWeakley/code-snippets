use mongodb::{bson::{ oid::ObjectId}};
use serde_json::json;

use crate::services::mongodb::crud::mongodb_crud_client::MongoDBCRUDClient;
use crate::services::mongodb::rest::rest_response::RESTResponse;

pub struct MongoDBRESTClient {
    crud_client: Option<MongoDBCRUDClient>,
}

impl MongoDBRESTClient {
    pub fn new(crud_client: MongoDBCRUDClient) -> Self {
        Self {
            crud_client: Some(crud_client),
        }
    }

    fn get_client(&self) -> &MongoDBCRUDClient {
        self.crud_client.as_ref().expect("MongoDB CRUD client is not set")
    }

    fn bad_request(&self, message: &str) -> RESTResponse {
        RESTResponse::new(400, "text/plain", message.to_string())
    }

    fn error_message(&self, message: &str) -> RESTResponse {
        RESTResponse::new(500, "text/plain", message.to_string())
    }

    fn not_found(&self, id: &ObjectId) -> RESTResponse {
        RESTResponse::new(404, "text/plain", format!("Error: Document with id {} not found", id))
    }

    fn ok(&self, data: serde_json::Value) -> RESTResponse {
        RESTResponse::new(200, "application/json", data.to_string())
    }

    fn created(&self, id: &ObjectId) -> RESTResponse {
        RESTResponse::new(201, "text/plain", id.to_string())
    }

    fn deleted(&self, id: &str) -> RESTResponse {
        RESTResponse::new(200, "text/plain", format!("Document with id {} deleted", id))
    }

    fn verify_parameters(
        &self,
        db_name: Option<&str>,
        collection_name: Option<&str>,
        id: Option<&str>,
        is_id_required: bool,
        data: Option<&serde_json::Value>,
        is_data_required: bool,
    ) -> Option<RESTResponse> {
        if db_name.is_none() {
            return Some(self.bad_request("Error: Missing db_name parameter"));
        }
        if collection_name.is_none() {
            return Some(self.bad_request("Error: Missing collection_name parameter"));
        }
        if is_id_required {
            if id.is_none() {
                return Some(self.bad_request("Error: Missing id parameter"));
            } else if !ObjectId::parse_str(id.unwrap()).is_ok() {
                return Some(self.bad_request("Error: Invalid id"));
            }
        }
        if is_data_required && data.is_none() {
            return Some(self.bad_request("Error: Missing body"));
        }
        None
    }

    pub async fn ping(&self) -> Result<(), mongodb::error::Error> {
        match self.get_client().ping().await {
            Ok(..) => Ok(()),
            Err(e) => Err(e),
        }
    }

    pub async fn get(
        &self,
        db_name: Option<&str>,
        collection_name: Option<&str>,
        id: Option<&str>,
    ) -> RESTResponse {
        if let Some(error) = self.verify_parameters(db_name, collection_name, id, true, None, false) {
            return error;
        }

        let client = self.get_client();
        match client.read(db_name.unwrap(), collection_name.unwrap(), ObjectId::parse_str(id.unwrap()).unwrap()).await {
            Ok(Some(document)) => self.ok(json!(document)),
            Ok(None) => self.not_found(&ObjectId::parse_str(id.unwrap()).unwrap()),
            Err(e) => self.error_message(&e.to_string()),
        }
    }

    pub async fn post(
        &self,
        db_name: Option<&str>,
        collection_name: Option<&str>,
        data: Option<serde_json::Value>,
    ) -> RESTResponse {
        if let Some(error) = self.verify_parameters(db_name, collection_name, None, false, data.as_ref(), true) {
            return error;
        }

        let client = self.get_client();
        match client.create(db_name.unwrap(), collection_name.unwrap(), data.unwrap()).await {
            Ok(document_id) => self.created(&document_id),
            //Ok(None) => self.error_message("Failed to create document"),
            Err(e) => self.error_message(&e.to_string()),
        }
    }

    pub async fn put(
        &self,
        db_name: Option<&str>,
        collection_name: Option<&str>,
        id: Option<&str>,
        data: Option<serde_json::Value>,
    ) -> RESTResponse {
        if let Some(error) = self.verify_parameters(db_name, collection_name, id, true, data.as_ref(), true) {
            return error;
        }

        let client = self.get_client();
        match client.update(db_name.unwrap(), collection_name.unwrap(), ObjectId::parse_str(id.unwrap()).unwrap(), data.unwrap()).await {
            Ok(Some(document)) => self.ok(json!(document)),
            Ok(None) => self.not_found(&ObjectId::parse_str(id.unwrap()).unwrap()),
            Err(e) => self.error_message(&e.to_string()),
        }
    }

    pub async fn delete(
        &self,
        db_name: Option<&str>,
        collection_name: Option<&str>,
        id: Option<&str>,
    ) -> RESTResponse {
        if let Some(error) = self.verify_parameters(db_name, collection_name, id, true, None, false) {
            return error;
        }

        let client = self.get_client();
        match client.delete(db_name.unwrap(), collection_name.unwrap(), ObjectId::parse_str(id.unwrap()).unwrap()).await {
            Ok(true) => self.deleted(id.unwrap()),
            Ok(false) => self.not_found(&ObjectId::parse_str(id.unwrap()).unwrap()),
            Err(e) => self.error_message(&e.to_string()),
        }
    }
}
