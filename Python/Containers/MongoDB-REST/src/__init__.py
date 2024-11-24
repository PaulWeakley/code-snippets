from flask import Flask, request, jsonify
import time
import configparser
from dotenv import load_dotenv
import os

from .services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from .services.MongoDB.CRUD.mongodb_crud_client import MongoDB_CRUD_Client
from .services.MongoDB.CRUD.mongodb_config import MongoDB_Config

from .controllers.health_controller import health_blueprint
from .controllers.mongodb_rest_controller import mongodb_rest_blueprint

app = Flask(__name__)

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

@app.before_request
def before_request():
    print("Before request")

@app.after_request
def after_request(response):
    print("After request")
    return response

def create_app(mongodb_config: MongoDB_Config):
    app.url_map.strict_slashes = False
    app.mongodb_rest_client = MongoDB_REST_Client(mongodb_crud_client=MongoDB_CRUD_Client(mongodb_config))

    app.register_blueprint(health_blueprint, url_prefix='/api/health')
    app.register_blueprint(mongodb_rest_blueprint, url_prefix='/api/mongodb')

    return app
