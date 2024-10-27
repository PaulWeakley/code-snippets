from flask import Flask, request, jsonify
import time
import configparser
from dotenv import load_dotenv
import os

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

mongodb_REST_client = MongoDB_REST_Client(mongodb_crud_client=MongoDB_CRUD_Client(mongodb_config))

app = Flask(__name__)

def build_health_check_mongodb():
    try:
        mongodb_REST_client.ping()
        return {
            'isHealthy': True,
            'responseTime': f"{round(time.time() % 1 * 1000)} ms"
        }
    except Exception as e:
        return {
            'isHealthy': False,
            'responseTime': f"{round(time.time() % 1 * 1000)} ms",
            'error': str(e)
        }
    

def build_health_check_response():
    mongo_health = build_health_check_mongodb()
    return {
        "status": "healthy" if mongo_health['isHealthy'] else "unhealthy",
        "details": {
            "mongodb": mongo_health
        },
        "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        "version": "1.0.0"
    }

@app.route('/health-check', methods=['GET', 'POST'])
def health_check():
    try: 
        mongo_health = build_health_check_mongodb()
        return jsonify({
            "status": "healthy" if mongo_health['isHealthy'] else "unhealthy",
            "details": {
                "mongodb": mongo_health
            },
            "timestamp": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
            "version": "1.0.0"
        })
    except Exception as e:
        return str(e), 500

@app.route('/<db_name>/<collection_name>/<id>', methods=['GET'])
def get(db_name, collection_name, id):
    response = mongodb_REST_client.get(db_name, collection_name, id)
    return response['body'], response['statusCode'], response.get('headers', {})

@app.route('/<db_name>/<collection_name>', methods=['POST'])
def post(db_name, collection_name):
    response = mongodb_REST_client.post(db_name, collection_name, request.get_json())
    return response['body'], response['statusCode'], response.get('headers', {})

@app.route('/<db_name>/<collection_name>/<id>', methods=['PUT', 'PATCH'])
def put(db_name, collection_name, id):
    response = mongodb_REST_client.put(db_name, collection_name, id, request.get_json())
    return response['body'], response['statusCode'], response.get('headers', {})

@app.route('/<db_name>/<collection_name>/<id>', methods=['DELETE'])
def delete(db_name, collection_name, id):
    response = mongodb_REST_client.delete(db_name, collection_name, id)
    return response['body'], response['statusCode'], response.get('headers', {})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)