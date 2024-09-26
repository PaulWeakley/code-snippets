import json
import unittest
from datetime import datetime
from cloud_event import CloudEvent

class TestCloudEvent(unittest.TestCase):

    def setUp(self):
        self.event = CloudEvent(
            specversion="1.0",
            type="com.example.someevent",
            source="/mycontext",
            subject="123",
            datacontenttype="application/json",
            data={"key": "value"}
        )

    def test_to_dict(self):
        expected_dict = {
            'specversion': "1.0",
            'type': "com.example.someevent",
            'source': "/mycontext",
            'subject': "123",
            'id': self.event.id,
            'time': self.event.time,
            'datacontenttype': "application/json",
            'data': {"key": "value"}
        }
        self.assertEqual(self.event.to_dict(), expected_dict)

    def test_to_json(self):
        expected_json = json.dumps({
            'specversion': "1.0",
            'type': "com.example.someevent",
            'source': "/mycontext",
            'subject': "123",
            'id': self.event.id,
            'time': self.event.time.isoformat() + "Z",
            'datacontenttype': "application/json",
            'data': {"key": "value"}
        })
        self.assertEqual(self.event.to_json(), expected_json)

    def test_str(self):
        expected_json = json.dumps({
            'specversion': "1.0",
            'type': "com.example.someevent",
            'source': "/mycontext",
            'subject': "123",
            'id': self.event.id,
            'time': self.event.time.isoformat() + "Z",
            'datacontenttype': "application/json",
            'data': {"key": "value"}
        })
        self.assertEqual(str(self.event), expected_json)

if __name__ == '__main__':
    unittest.main()