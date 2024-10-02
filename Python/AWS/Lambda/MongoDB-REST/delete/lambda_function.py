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
    id = None
    
    if event is not None:
        if "pathParameters" in event:
            pathParameters = event["pathParameters"]
            if pathParameters is not None:
                if "db_name" in pathParameters:
                    db_name = pathParameters["db_name"]
                if "collection_name" in pathParameters:
                    collection_name = pathParameters["collection_name"]
                if "id" in pathParameters:
                    id = pathParameters["id"]
    
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
    if id is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing id parameter')
        }
    elif not ObjectId.is_valid(id):
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Invalid id')
        }
            
    try:
        client = MongoDB_CRUD_Client(config=mongodb_config)
        result = client.delete(db_name, collection_name, ObjectId(id))
        if result:
            return {
                'statusCode': 200,
                'body': f'Document with id {id} deleted'
            }
        return {
            'statusCode': 404,
            'body': json.dumps(f'Error: Document with id {id} not found')
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': str(e)
        }
