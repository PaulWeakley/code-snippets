from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi
from bson.objectid import ObjectId

class MongoDB_CRUD_Client:
    def __init__(self, config):
        self.config = config
        self.uri = f"mongodb+srv://{config['username']}:{config['password']}@{config['server']}/?retryWrites={config['retryWrites']}&w={config['w']}&appName={config['appName']}"
        self.client = None
    
    def __get_client(self):
        if self.client is None:
            self.client = MongoClient(self.uri, server_api=ServerApi('1'))
        return self.client

    def ping(self):
        self.__get_client().admin.command('ping')

    def close(self):
        if self.client is not None:
            self.client.close()
            self.client = None

    def get_collection(self, db_name, collection_name):
        client = self.__get_client()
        return client[db_name][collection_name]
    
    def create(self, db_name, collection_name, document) -> ObjectId:
        collection = self.get_collection(db_name, collection_name)
        result = collection.insert_one(document)
        return result.inserted_id

    def read(self, db_name, collection_name, document_id: ObjectId):
        collection = self.get_collection(db_name, collection_name)
        document = collection.find_one({"_id": document_id})
        return document

    def update(self, db_name, collection_name, document_id: ObjectId, update_fields):
        collection = self.get_collection(db_name, collection_name)
        result = collection.find_one_and_update({"_id": document_id}, {"$set": update_fields}, new=True)
        return result

    def delete(self, db_name, collection_name, document_id: ObjectId):
        collection = self.get_collection(db_name, collection_name)
        result = collection.delete_one({"_id": document_id})
        return result.deleted_count == 1