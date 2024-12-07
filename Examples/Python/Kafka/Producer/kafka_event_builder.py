import sys
import os

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(os.path.join(current_dir, '../'))
from kafka_message import KafkaMessage

class IKafkaEventBuilder():
    def build_event(self, data: any, headers: dict = None) -> KafkaMessage:
        pass