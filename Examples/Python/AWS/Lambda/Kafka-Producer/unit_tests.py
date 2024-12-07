import unittest
import json
from unittest.mock import patch
from lambda_function import lambda_handler

class TestLambdaHandler(unittest.TestCase):

    def setUp(self):
        self.context = {}
        self.valid_event = {
            "pathParameters": {
                "topic": "test_topic",
                "subject": "test_subject"
            },
            "body": json.dumps({"key": "value"})
        }

    def test_missing_topic_parameter(self):
        event = {
            "pathParameters": {
                "subject": "test_subject"
            },
            "body": json.dumps({"key": "value"})
        }
        response = lambda_handler(event, self.context)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing topic parameter')

    def test_missing_body(self):
        event = {
            "pathParameters": {
                "topic": "test_topic",
                "subject": "test_subject"
            },
            "body": None
        }
        response = lambda_handler(event, self.context)
        self.assertEqual(response['statusCode'], 400)
        self.assertEqual(json.loads(response['body']), 'Error: Missing body')

    @patch('lambda_function.KafkaProducer')
    def test_valid_input(self, MockKafkaProducer):
        mock_producer_instance = MockKafkaProducer.return_value
        mock_producer_instance.produce_messages.return_value = None

        response = lambda_handler(self.valid_event, self.context)
        self.assertEqual(response['statusCode'], 204)
        mock_producer_instance.produce_messages.assert_called_once()
        mock_producer_instance.produce_messages.assert_called_once_with(
            topic='test_topic', subject="test_subject", 
            messages=[{"key": "value"}],
            callback=None
        )

if __name__ == '__main__':
    unittest.main()