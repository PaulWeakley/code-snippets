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

    def test_event_no_body(self):
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users' } }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing body')

    @patch('lambda_function.MongoDB_CRUD_Client')
    def test_valid_event(self, MockMongoDB_CRUD_Client):
        client_instance = MockMongoDB_CRUD_Client.return_value
        document_id = ObjectId()
        client_instance.create.return_value = document_id
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users' },
                  'body': json.dumps({'name': 'John Doe', 'email': ''}) }
        
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 201)
        self.assertEqual(response['body'], str(document_id))

    @patch('lambda_function.MongoDB_CRUD_Client')
    def test_valid_event_not_created(self, MockMongoDB_CRUD_Client):
        client_instance = MockMongoDB_CRUD_Client.return_value
        client_instance.create.return_value = None
        event = { 'pathParameters': {'db_name': 'test', 'collection_name': 'users' },
                  'body': json.dumps({'name': 'John Doe', 'email': ''}) }
        response = lambda_handler(event, None)
        self.assertEqual(response['statusCode'], 500)
        self.assertEqual(response['body'], 'Failed to create document')


if __name__ == '__main__':
    unittest.main()