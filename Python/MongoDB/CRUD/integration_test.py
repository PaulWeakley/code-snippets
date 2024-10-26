import configparser
import json
from dotenv import load_dotenv
import os
import unittest
import json

from mongodb_crud_client import MongoDB_CRUD_Client

class IntegrationTestMongoDBCRUDClient(unittest.TestCase):

    def setUp(self):
        # Load sensitive configuration values from config.ini
        config = configparser.ConfigParser()
        config.read('config.ini')

        load_dotenv()

        self.config = {
            'server': os.getenv('MONGODB_SERVER'),
            'username': os.getenv('MONGODB_USERNAME'),
            'password': os.getenv('MONGODB_PASSWORD'),
            'retryWrites': config.get('MongoDB', 'retryWrites'),
            'w': config.get('MongoDB', 'w'),
            'appName': config.get('MongoDB', 'appName')
        }

    def test_ping(self):
        mongodb_crud_client = MongoDB_CRUD_Client(config=self.config)
        mongodb_crud_client.ping()
        mongodb_crud_client.close()

    def test_crud(self):
        mongodb_crud_client = MongoDB_CRUD_Client(config=self.config)
        
        document = {
            "name": "John Doe",
            "email": ""
        }
        # Create
        document_id = mongodb_crud_client.create("test", "users", document)
        assert document_id is not None
        # Read
        document_compare = mongodb_crud_client.read("test", "users", document_id)
        assert document_compare is not None
        assert document_compare == document
        # Update
        document_compare = mongodb_crud_client.update("test", "users", document_id, {"email": "test@example.com"})
        document = {
            "name": "John Doe",
            "email": "test@example.com"
        }
        del document_compare["_id"]
        assert document_compare == document
        # Delete
        assert mongodb_crud_client.delete("test", "users", document_id)
        # Close
        mongodb_crud_client.close()

if __name__ == '__main__':
    unittest.main()