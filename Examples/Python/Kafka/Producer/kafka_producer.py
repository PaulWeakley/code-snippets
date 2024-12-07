import sys
import os
from typing import Callable



# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../Interop/'))
sys.path.append(os.path.join(current_dir, '../../Kafka/'))

from confluent_kafka import Producer
from interop_serializer import InteropSerializer
from cloud_event import CloudEvent
from kafka_message import KafkaMessage
from kafka_event_builder import IKafkaEventBuilder

class KafkaProducer:
    def __init__(self, config):
        self.__config = config
        self.__producer = Producer(config)

    def produce_messages(self, topic: str, event_builder: IKafkaEventBuilder, messages: list,
                         headers: dict=None, 
                         callback: Callable[[any, any], None]=None) -> bool:
        for message in messages:
            kafka_message = event_builder.build_event(data=message, headers=headers)
            try:
                # Produce a message
                self.__produce(topic=topic, kafka_message=kafka_message, callback=callback)
                self.__producer.poll(1)  # Poll to handle events (delivery reports, etc.)
            except BufferError:
                print("Local producer queue is full. Waiting to send message...")
                self.__producer.flush()  # Wait for messages to be delivered
                self.__produce(topic=topic, kafka_message=kafka_message, callback=callback)
                
        # Wait for all messages in the producer queue to be delivered
        self.__producer.flush()
        return True

    def __produce(self, topic: str, kafka_message: KafkaMessage, callback: Callable[[any, any], None]=None):
        self.__producer.produce(topic, value=kafka_message.value, key=kafka_message.key, callback=callback)

    def close(self):
        self.__producer.flush()
        self.__producer.close()
        print("Kafka producer closed")