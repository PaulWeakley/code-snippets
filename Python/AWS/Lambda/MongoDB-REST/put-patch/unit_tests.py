import unittest
from unittest.mock import patch
import json

from bson import ObjectId
from lambda_function import lambda_handler

class TestLambdaHandler(unittest.TestCase):

    def test_event_none(self):
        event = None
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing db_name parameter') 
     
    def test_event_no_path_parameters(self):
        event = {}
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing db_name parameter')
    
    def test_event_no_collection_name(self):
        event = { 'pathParameters': {'db_name': 'test' } }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing collection_name parameter') 

    def test_event_no_id(self):
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users' } }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing id parameter')

    def test_event_invalid_id(self):
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users', 'id': '123' } }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Invalid id')

    def test_event_no_body(self):
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users', 'id': str(ObjectId()) } }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing body')

    @patch('lambda_function.MongoDB_CRUD_Client')
    def test_valid_event(self, MockMongoDB_CRUD_Client):
        client_instance = MockMongoDB_CRUD_Client.return_value
        document_id = ObjectId()
        client_instance.update.return_value = {'name': 'John Doe', 'email': ''}
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users', 'id': str(ObjectId()) },
                  'body': json.dumps({'name': 'John Doe', 'email': ''}) }
        
        response = lambda_handler(event, client_instance.update.return_value)
        self.assertEqual(response['statusCode'], 200)
        self.assertEqual(json.loads(response['body']), {'name': 'John Doe', 'email': ''})

    @patch('lambda_function.MongoDB_CRUD_Client')
    def test_valid_event_not_created(self, MockMongoDB_CRUD_Client):
        client_instance = MockMongoDB_CRUD_Client.return_value
        client_instance.update.return_value = None
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users', 'id': str(ObjectId()) },
                  'body': json.dumps({'name': 'John Doe', 'email': ''}) }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 404)
        self.assertEqual(response['body'], f'Error: Document with id {event["pathParameters"]["id"]} not found')


if __name__ == '__main__':
    unittest.main()