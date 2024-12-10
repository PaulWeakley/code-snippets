import configparser
import os
from dotenv import load_dotenv
from flask import Flask

from .services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from .services.MongoDB.CRUD.mongodb_crud_client import MongoDB_CRUD_Client
from .services.MongoDB.CRUD.mongodb_config import MongoDB_Config

from .controllers.health_controller import health_blueprint
from .controllers.mongodb_rest_controller import mongodb_rest_blueprint

# Load environment variables
config = configparser.ConfigParser()
config.read('config.ini')
load_dotenv()

mongodb_config = MongoDB_Config(
    server=os.getenv('MONGODB_SERVER'),
    username=os.getenv('MONGODB_USERNAME'),
    password=os.getenv('MONGODB_PASSWORD'),
    retryWrites=config.get('MongoDB', 'retryWrites'),
    w=config.get('MongoDB', 'w'),
    appName=config.get('MongoDB', 'appName')
)

def create_app():
    app = Flask(__name__)

    app.url_map.strict_slashes = False
    app.mongodb_rest_client = MongoDB_REST_Client(mongodb_crud_client=MongoDB_CRUD_Client(mongodb_config))

    app.register_blueprint(health_blueprint, url_prefix='/api/health')
    app.register_blueprint(mongodb_rest_blueprint, url_prefix='/api/mongodb')

    return app
