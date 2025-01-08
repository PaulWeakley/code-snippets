import os
from dotenv import load_dotenv
from flask import Flask

from .services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from .services.MongoDB.CRUD.mongodb_crud_client import MongoDB_CRUD_Client
from .services.MongoDB.CRUD.mongodb_config import MongoDB_Config

from .controllers.health_controller import health_blueprint
from .controllers.mongodb_rest_controller import mongodb_rest_blueprint

# Load environment variables
load_dotenv()

mongodb_config = MongoDB_Config(
    server=os.getenv('MONGODB_SERVER'),
    username=os.getenv('MONGODB_USERNAME'),
    password=os.getenv('MONGODB_PASSWORD'),
    retryWrites=os.getenv('MONGODB_RETRY_WRITES'),
    writeConcern=os.getenv('MONGODB_WRITE_CONCERN'),
    appName=os.getenv('MONGODB_APP_NAME'),
    minPoolSize=os.getenv('MONGODB_MIN_POOL_SIZE'),
    maxPoolSize=os.getenv('MONGODB_MAX_POOL_SIZE'),
    waitQueueTimeoutMS=os.getenv('MONGODB_WAIT_QUEUE_TIMEOUT_MS')
)

def create_app():
    app = Flask(__name__)

    app.url_map.strict_slashes = False
    MongoDB_CRUD_Client.set_config(mongodb_config)
    app.mongodb_rest_client = MongoDB_REST_Client()

    app.register_blueprint(health_blueprint, url_prefix='/api/health')
    app.register_blueprint(mongodb_rest_blueprint, url_prefix='/api/mongodb')

    return app
