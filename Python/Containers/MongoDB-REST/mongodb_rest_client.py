import json
from bson import ObjectId
from typing import Optional, Any

from mongodb_crud_client import MongoDB_CRUD_Client

class MongoDB_REST_Client:
    def __init__(self, mongodb_crud_client: MongoDB_CRUD_Client):
        self.__crud_client = mongodb_crud_client

    def __bad_request(self, message: str) -> dict:
        return {
            'statusCode': 400,
            'headers': {'Content-Type': 'text/plain'},
            'body': json.dumps(message)
        }

    def __error_message(self, e: Exception) -> dict:
        return {
            'statusCode': 500,
            'headers': {'Content-Type': 'text/plain'},
            'body': str(e)
        }

    def __not_found(self, id: ObjectId) -> dict:
        return {
            'statusCode': 404,
            'headers': {'Content-Type': 'text/plain'},
            'body': f'Error: Document with id {id} not found'
        }

    def __ok(self, data: dict) -> dict:
        return {
            'statusCode': 200,
            'headers': {'Content-Type': 'application/json'},
            'body': json.dumps(data, default=str)
        }
    
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
    
    def ping(self):
        self.__crud_client.ping()

    def get(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str]) -> dict: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, None, False)
            if error is not None:
                return error
            
            document = self.__crud_client.read(db_name, collection_name, ObjectId(id))
            if document is not None:
                return self.__ok(document)
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)
        
    def post(self, db_name: Optional[str], collection_name: Optional[str], data: Optional[any]) -> dict:
        try:
            error = self.__verify_parameters(db_name, collection_name, None, False, data, True)
            if error is not None:
                return error
            
            document_id = self.__crud_client.create(db_name, collection_name, data)
            if document_id is not None:
                return {
                    'statusCode': 201,
                    'body': str(document_id)
                }
            return {
                'statusCode': 500,
                'body': 'Failed to create document'
            }
        except Exception as e:
            return self.__error_message(e)

    def put(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str], data: Optional[any]) -> dict: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, data, True)
            if error is not None:
                return error
            
            document = self.__crud_client.update(db_name, collection_name, ObjectId(id), data)
            if document is not None:
                return self.__ok(document)
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)

    def delete(self, db_name: Optional[str], collection_name: Optional[str], id: Optional[str]) -> dict: 
        try:
            error = self.__verify_parameters(db_name, collection_name, id, True, None, False)
            if error is not None:
                return error
            
            result = self.__crud_client.delete(db_name, collection_name, ObjectId(id))
            if result:
                return {
                    'statusCode': 200,
                    'body': f'Document with id {id} deleted'
                }
            return self.__not_found(id)
        except Exception as e:
            return self.__error_message(e)

