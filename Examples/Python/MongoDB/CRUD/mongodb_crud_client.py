from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi
from bson import ObjectId
from mongodb_config import MongoDB_Config

class MongoDB_CRUD_Client:
    def __init__(self, config: MongoDB_Config):
        self.__uri = config.to_uri()
        self.__client = None
    
    def __get_client(self):
        if self.__client is None:
            self.__client = MongoClient(self.__uri, server_api=ServerApi('1'))
        return self.__client

    def ping(self):
        self.__get_client().admin.command('ping')

    def close(self):
        if self.__client is not None:
            self.__client.close()
            self.__client = None

    def __get_collection(self, db_name, collection_name):
        client = self.__get_client()
        return client[db_name][collection_name]
    
    def create(self, db_name, collection_name, document) -> ObjectId:
        collection = self.__get_collection(db_name, collection_name)
        result = collection.insert_one(document)
        return result.inserted_id

    def read(self, db_name, collection_name, document_id: ObjectId):
        collection = self.__get_collection(db_name, collection_name)
        document = collection.find_one({"_id": document_id})
        return document

    def update(self, db_name, collection_name, document_id: ObjectId, update_fields):
        collection = self.__get_collection(db_name, collection_name)
        result = collection.find_one_and_update({"_id": document_id}, {"$set": update_fields}, new=True)
        return result

    def delete(self, db_name, collection_name, document_id: ObjectId):
        collection = self.__get_collection(db_name, collection_name)
        result = collection.delete_one({"_id": document_id})
        return result.deleted_count == 1