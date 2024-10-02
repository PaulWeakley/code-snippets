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


class KafkaProducer:
    def __init__(self, config, use_interop=True):
        self.__config = config
        self.__producer = Producer(config)
        self.__use_interop = use_interop

    # Function to produce messages to a Kafka topic
    def produce_messages(self, topic: str, subject: str, messages: list, callback: Callable[[any, any], None]=None):
        for message in messages:
            event = self.__to_cloud_event(source=self.__config['client.id'], 
                                          subject=subject, data=message, 
                                          use_interop=self.__use_interop)
            try:
                # Produce a message
                self.__produce(topic=topic, subject=subject, event=event, callback=callback)
                self.__producer.poll(1)  # Poll to handle events (delivery reports, etc.)
            except BufferError:
                print("Local producer queue is full. Waiting to send message...")
                self.__producer.flush()  # Wait for messages to be delivered
                self.__produce(topic=topic, subject=subject, event=event, callback=callback)
                
        # Wait for all messages in the producer queue to be delivered
        self.__producer.flush()

    def __produce(self, topic: str, subject: str, event: CloudEvent, callback: Callable[[any, any], None]=None):
        # Produce a message
        key = subject.encode('utf-8') if subject is not None else None
        self.__producer.produce(topic, value=event.to_json().encode('utf-8'), key=key, callback=callback)

    def __to_cloud_event(self, spec_version=None, data_type=None, source=None, subject=None, data=None, use_interop=True):
        spec_version = spec_version if spec_version is not None else "1.0"
        data_type = data_type if data_type is not None else ""
        subject = subject if subject is not None else ""
        
        if isinstance(data, dict):
            if use_interop:
                data_content_type = "application/interop"
                serialized_data = InteropSerializer.serialize(data)
            else:
                data_content_type = "application/json"
                serialized_data = data
        else:
            data_content_type = "text/plain"
            serialized_data = data
            
        return CloudEvent(
            specversion=spec_version, 
            type=data_type, 
            source=source, 
            subject=subject,  
            datacontenttype=data_content_type, 
            data=serialized_data
            )