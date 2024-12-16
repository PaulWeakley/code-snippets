import json
from bson import ObjectId
import os
import sys
from typing import Optional, Any

from src.services.MongoDB.CRUD.mongodb_crud_client import MongoDB_CRUD_Client
from src.services.MongoDB.CRUD.mongodb_config import MongoDB_Config
from .rest_response import REST_Response

class MongoDB_REST_Client:
    def __init__(self, mongodb_config: MongoDB_Config):
        self.__mongodb_config = mongodb_config
        self.__crud_client = None

    def __bad_request(self, message: str) -> REST_Response:
        return REST_Response(400, 'text/plain', message)

    def __error_message(self, e: Exception) -> REST_Response:
        return self.__error_message(str(e))
    
    def __error_message(self, message: str) -> REST_Response:
        return REST_Response(500, 'text/plain', message)

    def __not_found(self, id: ObjectId) -> REST_Response:
        return REST_Response(404, 'text/plain', f'Error: Document with id {id} not found')

    def __ok(self, data: dict) -> REST_Response:
        return REST_Response(200, 'application/json', json.dumps(data, default=str))
    
    def __created(self, id: ObjectId) -> REST_Response:
        return REST_Response(201, 'text/plain', str(id))
    
    def __deleted(self, id: str) -> REST_Response:
        return REST_Response(200, 'text/plain', f'Document with id {id} deleted')
    
    def __verify_parameters(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str], is_id_required: bool, data: Optional[Any], is_data_required: bool) -> Optional[dict]:
        if db_name is None:
            return self.__bad_request('Error: Missing db_name parameter')
        if collection_name is None:
            return self.__bad_request('Error: Missing collection_name parameter')
        if is_id_required:
            if id is None:
                return self.__bad_request('Error: Missing id parameter')
            elif not ObjectId.is_valid(id):
                return self.__bad_request('Error: Invalid id')
        if is_data_required and data is None:
            return self.__bad_request('Error: Missing body')
        return None
    
    def __get_client(self) -> MongoDB_CRUD_Client:
        if self.__crud_client is None:
            self.__crud_client = MongoDB_CRUD_Client(self.__mongodb_config)
        return self.__crud_client
    
    def ping(self):
        with self.__get_client() as client:
            client.ping()

    def get(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str]) -> REST_Response: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, None, False)
            if error is not None:
                return error
            
            with self.__get_client() as client:
                document = client.read(db_name, collection_name, ObjectId(id))
            if document is not None:
                return self.__ok(document)
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)
        
    def post(self, db_name: Optional[str], collection_name: Optional[str], data: Optional[any]) -> REST_Response:
        try:
            error = self.__verify_parameters(db_name, collection_name, None, False, data, True)
            if error is not None:
                return error
            
            with self.__get_client() as client:
                document_id = client.create(db_name, collection_name, data)
            if document_id is not None:
                return self.__created(document_id)
            return self.__error_message('Failed to create document')
        except Exception as e:
            return self.__error_message(e)

    def put(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str], data: Optional[any]) -> REST_Response: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, data, True)
            if error is not None:
                return error
            
            with self.__get_client() as client:
                document = client.update(db_name, collection_name, ObjectId(id), data)
            if document is not None:
                return self.__ok(document)
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)

    def delete(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str]) -> REST_Response: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, None, False)
            if error is not None:
                return error
            
            with self.__get_client() as client:
                result = client.delete(db_name, collection_name, ObjectId(id))
            if result:
                return self.__deleted(id)
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)

