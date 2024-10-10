import configparser
from dotenv import load_dotenv
import os
from json_cloud_event_builder import JsonCloudEventBuilder
from kafka_producer import KafkaProducer

def __main__():
    # Load sensitive configuration values from config.ini
    config = configparser.ConfigParser()
    config.read('config.ini')

    load_dotenv()

    conf = {
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

    producer = KafkaProducer(config=conf)
    producer.produce_messages(topic='topic_0', event_builder=JsonCloudEventBuilder(), messages=[{"key": "value"}],
                              callback=delivery_report)

# Delivery report callback to confirm if the message was successfully delivered or failed
def delivery_report(err, msg):
    if err is not None:
        print(f"Message delivery failed: {err}")
    else:
        print(f"Message delivered to {msg.topic()} [{msg.partition()}] at offset {msg.offset()}")

__main__()