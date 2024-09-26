import sys
import os
from typing import Callable

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../Interop/'))
sys.path.append(os.path.join(current_dir, '../../Kafka/'))

from confluent_kafka import Consumer, KafkaError
from cloud_event import CloudEvent

class KafkaConsumer:
    
    def __init__(self, config: any, topic: str, callback: Callable[[str, CloudEvent], bool]):
        self.__consumer = Consumer(config)
        self.__consumer.subscribe([topic])
        self.__callback = callback

    def consume_messages(self):
        try:
            while True:
                # Poll for messages
                message = self.__consumer.poll(1.0)

                if message is None:
                    continue
                
                if message.error():
                    if message.error().code() == KafkaError._PARTITION_EOF:
                        # End of partition event
                        continue
                    else:
                        print(f"Error: {message.error()}")
                        raise Exception(message.error())

                # Access the message value and key
                message_value = message.value().decode('utf-8') if message.value() else None
                message_key = message.key().decode('utf-8') if message.key() else None

                # Acknowledge the message (commit the offset manually)
                if self.__callback(message_key, CloudEvent.from_json(message_value)):
                    self.__consumer.commit(asynchronous=False)  # Set `asynchronous=True` for async commit
                else:
                    print(f'Message processing failed. Subject: {message_key}. Skipping commit...')

        finally:
            # Ensure the consumer is properly closed
            self.__consumer.close()