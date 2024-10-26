import json
import configparser
from bson import ObjectId
from dotenv import load_dotenv
import os
import sys

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../../../MongoDB/CRUD'))
sys.path.append(os.path.join(current_dir, '../../../../MongoDB/REST'))
from mongodb_crud_client import MongoDB_CRUD_Client
from mongodb_rest_client import MongoDB_REST_Client

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
    
    return MongoDB_REST_Client(mongodb_crud_client=MongoDB_CRUD_Client(mongodb_config)).get(db_name, collection_name, id)
