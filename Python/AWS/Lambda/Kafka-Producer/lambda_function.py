import json
import configparser
from dotenv import load_dotenv
import os
import sys

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../../Kafka/Producer/'))
from kafka_producer import KafkaProducer # type: ignore

config = configparser.ConfigParser()
config.read('config.ini')

load_dotenv()

producer_config = {
    'bootstrap.servers': os.getenv('KAFKA_BOOTSTRAP_SERVERS'),
    'client.id': config.get('Kafka', 'client.id'),
    'security.protocol': config.get('Kafka', 'security.protocol'),
    'sasl.mechanisms': config.get('Kafka', 'sasl.mechanisms'),
    'sasl.username': os.getenv('KAFKA_PRODUCER_SASL_USERNAME'),
    'sasl.password': os.getenv('KAFKA_PRODUCER_SASL_PASSWORD'),
    'acks': config.get('Kafka', 'acks'),
    'retries': config.getint('Kafka', 'retries'),
    'compression.type': config.get('Kafka', 'compression.type'),
}

def lambda_handler(event, context):
    topic = None
    subject = None
    data = None
    use_interop = False

    if event is not None:
        if "pathParameters" in event:
            pathParameters = event["pathParameters"]
            if pathParameters is not None:
                if "topic" in pathParameters:
                    topic = pathParameters["topic"]
                if "subject" in pathParameters:
                    subject = pathParameters["subject"]
        if "body" in event and event["body"] is not None:
            data = json.loads(event["body"])
        if "queryStringParameters" in event and event["queryStringParameters"] is not None:
            queryStringParameters = event["queryStringParameters"]
            if "interop" in queryStringParameters and queryStringParameters["interop"] is not None:
                use_interop = queryStringParameters["interop"].lower() in ['true', '1', 'yes', 'on']

    
    if topic is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing topic parameter')
        }
    
    if data is None:
        return {
            'statusCode': 400,
            'body': json.dumps('Error: Missing body')
        }
    
    try:
        producer = KafkaProducer(config=producer_config)
        producer.produce_messages(topic=topic, event_builder=None, messages=[data], headers=None, callback=None)
        return {
            'statusCode': 204,
        }
    except Exception as e:
        return {
            'statusCode': 500,
            'body': str(e)
        }
