import sys
import os
import json
from cloud_event_builder import CloudEventBuilder
from cloud_event_headers import CloudEventHeaders

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(os.path.join(current_dir, '../'))
from kafka_message import KafkaMessage

class JsonCloudEventBuilder(CloudEventBuilder):
    def build_event(self, data: any, headers: dict = None) -> KafkaMessage:
        default_headers = CloudEventHeaders.extract_default_headers(headers)
        cloud_event = self.to_cloud_event(specversion=default_headers['specversion'],
                                          type=default_headers['type'], 
                                          source=default_headers['source'], 
                                          subject=default_headers['subject'], 
                                          data=data)
        return KafkaMessage(key=cloud_event.id, value=json.dumps(cloud_event, default=str).encode('utf-8'))
    
    def __get_data_content_type(self) -> str:
        return "application/json"