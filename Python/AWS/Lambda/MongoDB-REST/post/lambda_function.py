import json
import configparser
import json
from bson import ObjectId
from dotenv import load_dotenv
import os
import sys
from typing import Callable

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../../../MongoDB/'))
from mongodb_crud_client import MongoDB_CRUD_Client

config = configparser.ConfigParser()
config.read('config.ini')

load_dotenv()

mongodb_config = {
    'server': os.getenv('MONGODB_SERVER'),
    'username': os.getenv('MONGODB_USERNAME'),
    'password': os.getenv('MONGODB_PASSWORD'),
    'retryWrites': config.get('MongoDB', 'retryWrites'),
    'w': config.get('MongoDB', 'w'),
    'appName': config.get('MongoDB', 'appName')
}

def lambda_handler(event, context):
    db_name = None
    collection_name = None
    data = None
    
    if event is not None:
        if "pathParameters" in event:
            pathParameters = event["pathParameters"]
            if pathParameters is not None:
                if "db_name" in pathParameters:
                    db_name = pathParameters["db_name"]
                if "collection_name" in pathParameters:
                    collection_name = pathParameters["collection_name"]
        if "body" in event and event["body"] is not None:
            data = json.loads(event["body"])
    
    if db_name is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing db_name parameter')
        }
    if collection_name is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing collection_name parameter')
        }
    if data is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing body')
        }
            
    try:
        client = MongoDB_CRUD_Client(config=mongodb_config)
        document_id = client.create(db_name, collection_name, data)
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
        return {
            'statusCode': 500,
            'body': str(e)
        }
