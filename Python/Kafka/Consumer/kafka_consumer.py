import sys
import os
import decimal
import threading
import logging

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))

# Add the directory containing interop.py to the system path
sys.path.append(os.path.join(current_dir, '../../Kafka/'))

from confluent_kafka import Consumer, KafkaError
from cloud_event import CloudEvent
from kafka_message import KafkaMessage
from kafka_event_handler import IKafkaEventHandler


class KafkaConsumer:
    
    def __init__(self, config: any, logger: logging.Logger):
        self.__config = config
        self.__thread = None
        self.__stop_event = None
        self.__logger = logger

    def start_consuming_async(self, topic: str, polling_interval: decimal = 1.0,  handler: IKafkaEventHandler = None):
        if self.is_stopping():
            raise Exception("Consumer is stopping. Please wait for the consumer to stop before starting it again.")
        if self.is_running():
            raise Exception("Consumer is already running.")
        self.__stop_event = threading.Event()
        self.__thread = threading.Thread(target=self.__run_consumer, args=(topic, polling_interval, handler))
        self.__thread.daemon = True
        self.__thread.start()

    def is_running(self):
        return None != self.__thread and self.__thread.is_alive()
    
    def is_stopping(self):
        return None != self.__stop_event and self.__stop_event.is_set()

    def stop_consuming(self):
        if None != self.__stop_event:
            self.__stop_event.set()
    
    def __run_consumer(self, topic: str, polling_interval: decimal = 1.0,  handler: IKafkaEventHandler = None):
        consumer = Consumer(self.__config)
        try:
            consumer.subscribe([topic])
            while not self.__stop_event.is_set():
                # Poll for messages
                message = consumer.poll(polling_interval)
                if message is None:
                    continue
                
                if message.error():
                    if message.error().code() == KafkaError._PARTITION_EOF:
                        # End of partition event
                        continue
                    else:
                        self.__logger.error(f"Error: {message.error()}")
                        raise Exception(message.error())

                # Acknowledge the message (commit the offset manually)
                if handler and handler.handle(message=KafkaMessage(key=message.key(), value=message.value())):
                    consumer.commit(asynchronous=False)  # Set `asynchronous=True` for async commit
                else:
                    self.__logger.error(f'Message processing failed. Subject: {message.key()}. Skipping commit...')

        finally:
            # Ensure the consumer is properly closed
            consumer.close()