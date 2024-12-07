from dataclasses import dataclass

@dataclass
class KafkaMessage:
    key: any
    value: any
