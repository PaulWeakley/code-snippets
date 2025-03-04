use mongodb::{bson::{ doc, to_bson, to_document, Document, oid::ObjectId}, Client, Collection};
use serde::{Serialize};

use mongodb::options::ClientOptions;
use mongodb::bson;

use crate::MongoDBConfig;

pub struct MongoDBCRUDClient {
    client: Option<Client>,
}

impl MongoDBCRUDClient {
    pub async fn new(config: MongoDBConfig) -> Self {
        let connection_string = config.to_uri();
        let client_options = ClientOptions::parse(connection_string).await.unwrap();
        let client = Client::with_options(client_options).unwrap();
        Self { client: Some(client) }
    }

    fn get_client(&self) -> &Client {
        self.client.as_ref().expect("MongoDB client is not set")
    }

    pub async fn ping(&self) -> Result<(), mongodb::error::Error> {
        match self.get_client().database("admin").run_command(doc! {"ping": 1}, None).await {
            Ok(..) => Ok(()),
            Err(e) => Err(e),
        }
    }

    fn get_collection(&self, db_name: &str, collection_name: &str) -> Collection<Document> {
        self.get_client().database(db_name).collection(collection_name)
    }

    pub async fn create(&self, db_name: &str, collection_name: &str, document: impl Serialize) -> Result<ObjectId, mongodb::error::Error> {
        let collection = self.get_collection(db_name, collection_name);
        match collection.insert_one(to_document(&document).unwrap(), None).await {
            Ok(result) => Ok(result.inserted_id.as_object_id().unwrap().clone()),
            Err(e) => Err(e),
        }
    }

    pub async fn read(&self, db_name: &str, collection_name: &str, document_id: ObjectId) -> Result<Option<bson::Document>, mongodb::error::Error> {
        let collection = self.get_collection(db_name, collection_name);
        match collection.find_one(doc! {"_id": document_id}, None).await {
            Ok(result) => Ok(result),
            Err(e) => Err(e),
        }
    }

    pub async fn update(&self, db_name: &str, collection_name: &str, document_id: ObjectId, update_fields: impl Serialize) -> Result<Option<bson::Document>, mongodb::error::Error> {
        let collection = self.get_collection(db_name, collection_name);
        match collection.find_one_and_update(doc! {"_id": document_id}, doc! {"$set": to_bson(&update_fields).unwrap()}, None).await {
            Ok(result) => Ok(result),
            Err(e) => Err(e),
        }
    }

    pub async fn delete(&self, db_name: &str, collection_name: &str, document_id: ObjectId) -> Result<bool, mongodb::error::Error> {
        let collection = self.get_collection(db_name, collection_name);
        match collection.delete_one(doc! {"_id": document_id}, None).await {
            Ok(result) => Ok(result.deleted_count == 1),
            Err(e) => Err(e),
        }
    }
}