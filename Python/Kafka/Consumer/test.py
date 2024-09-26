import sys
import os
from typing import Callable

import configparser
from dotenv import load_dotenv
# Add the directory containing interop.py to the system path
sys.path.append(os.path.abspath('./../../Interop/'))
sys.path.append(os.path.abspath('./../../Kafka/'))

import json
from interop_serializer import InteropSerializer
from cloud_event import CloudEvent
from kafka_consumer import KafkaConsumer

def __main__():
    # Load sensitive configuration values from config.ini
    config = configparser.ConfigParser()
    config.read('config.ini')

    load_dotenv()

    conf = {
    'bootstrap.servers': os.getenv('KAFKA_BOOTSTRAP_SERVERS'),
    'group.id': config.get('Kafka', 'group.id'),
    'security.protocol': config.get('Kafka', 'security.protocol'),
    'sasl.mechanisms': config.get('Kafka', 'sasl.mechanisms'),
    'sasl.username': os.getenv('KAFKA_CONSUMER_SASL_USERNAME'),
    'sasl.password': os.getenv('KAFKA_CONSUMER_SASL_PASSWORD'),
    'auto.offset.reset': config.get('Kafka', 'auto.offset.reset', fallback='earliest'),  # Start reading from earliest message
    'enable.auto.commit': config.get('Kafka', 'enable.auto.commit', fallback=False)       # Disable automatic offset committing
    }
    
    consumer = KafkaConsumer(config=conf, topic='topic_0', callback=event_handler)
    
    consumer.consume_messages()

def event_handler(subject: str, event: CloudEvent):
    if event is None:
        print(f"Received message with no event data. Key: {subject}. Eventually send to dead letter queue.")
        return True
    try:
        if event.data is not None:
            if event.datacontenttype == 'application/interop':
                message_value = InteropSerializer.deserialize(event.data)
            elif event.datacontenttype == 'application/json':
                message_value = json.loads(event.data)
            else:
                message_value = event.data

                    # Process the message (add your processing logic here)
        print(f"Received message: {message_value}, Key: {subject}")
        return True
    except Exception as e:
        print(f"An error occurred while processing the message: {e}")

    return False


__main__()