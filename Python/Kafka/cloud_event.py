import json
import uuid
from dataclasses import dataclass
from datetime import datetime
from datetime import timezone

@dataclass
class CloudEvent:
    def __init__(self, specversion: str, type: str, source: str, subject: str, 
                         datacontenttype: str, data: any):
        self.specversion = specversion
        self.type = type
        self.source = source
        self.subject = subject
        self.id = str(uuid.uuid4())
        self.time = datetime.now(timezone.utc)
        self.datacontenttype = datacontenttype
        self.data = data
            
    specversion: str
    type: str
    source: str
    subject: str
    id: str
    time: datetime
    datacontenttype: str
    data: any
    
    def to_dict(self):
        return {
            'specversion': self.specversion,
            'type': self.type,
            'source': self.source,
            'subject': self.subject,
            'id': self.id,
            'time': self.time,
            'datacontenttype': self.datacontenttype,
            'data': self.data
        }
    
    def to_json(self):
        return json.dumps(self.to_dict(), default=default)

    def __str__(self):
        return self.to_json()
    
    @classmethod
    def from_json(cls, json_str):
        if not json_str or json_str.isspace():
            return None
        data = json.loads(json_str)
        id = data['id']
        time = datetime.fromisoformat(data['time'].replace('Z', ''))
        del data['id']
        del data['time']
        event = cls(**data)
        event.id = id
        event.time = time
        return event

def default(obj):
    if isinstance(obj, datetime):
        return obj.isoformat() + "Z"
    raise TypeError(f"Object of type {obj.__class__.__name__} is not JSON serializable")