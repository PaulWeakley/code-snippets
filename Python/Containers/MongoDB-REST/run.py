import configparser
from dotenv import load_dotenv
import os

from src import create_app
from src.services.MongoDB.CRUD.mongodb_config import MongoDB_Config

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

app = create_app(mongodb_config)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)