import sys
import os
from kafka_event_builder import IKafkaEventBuilder

# Get the directory of the current file
current_dir = os.path.dirname(os.path.abspath(__file__))
sys.path.append(os.path.join(current_dir, '../'))
from cloud_event import CloudEvent
from kafka_message import KafkaMessage

class CloudEventBuilder(IKafkaEventBuilder):
    def build_event(self, data: any) -> KafkaMessage:
        pass

    def __get_data_content_type(self) -> str:
        pass

    def to_cloud_event(self, specversion: str, type: str, source: str, subject: str, data: any) -> CloudEvent:
        return CloudEvent(
            specversion=specversion,
            type=type,
            source=source,
            subject=subject,
            datacontenttype=self.__get_data_content_type(),
            data=data
        )