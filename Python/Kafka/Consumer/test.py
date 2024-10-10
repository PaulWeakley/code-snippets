import sys
import os
import logging
import threading

import configparser
from dotenv import load_dotenv


# Add the directory containing interop.py to the system path
sys.path.append(os.path.abspath('./../../Interop/'))
sys.path.append(os.path.abspath('./../../Kafka/'))

from kafka_message import KafkaMessage
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

    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')
    logger = logging.getLogger(__name__)
    
    consumer = KafkaConsumer(config=conf, logger=logger)
    
    consumer.start_consuming_async(topic='topic_0', polling_interval=5, handler=KafkaEventHandler(logger))
    threading.Event().wait(15)
    consumer.stop_consuming()


class KafkaEventHandler:
    def __init__(self, logger) -> None:
        self.__logger = logger

    def handle(self, message: KafkaMessage) -> bool:
        self.__logger.info(f"Received message: {message}\n")
        return True

__main__()