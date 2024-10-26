import json
import time
import boto3
from aws_xray_sdk.core import xray_recorder
from aws_xray_sdk.core import patch_all

# Patch all supported libraries for X-Ray
patch_all()
# Ensure the log stream exists
def ensure_log_stream_exists(log_group_name, log_stream_name):
    try:
        cloudwatch.create_log_stream(
            logGroupName=log_group_name,
            logStreamName=log_stream_name
        )
    except cloudwatch.exceptions.ResourceAlreadyExistsException:
        pass

# Ensure the log group exists
def ensure_log_group_exists(log_group_name):
    try:
        cloudwatch.create_log_group(logGroupName=log_group_name)
    except cloudwatch.exceptions.ResourceAlreadyExistsException:
        pass


# Initialize CloudWatch client
cloudwatch = boto3.client('logs')

def lambda_handler(event, context):
    # Start a segment for X-Ray
    segment = xray_recorder.begin_segment('lambda_handler')

    # Log to CloudWatch
    # Ensure the log group and log stream exist
    log_group_name = '/aws/lambda/' + context.function_name
    log_stream_name = context.aws_request_id

    ensure_log_group_exists(log_group_name)
    ensure_log_stream_exists(log_group_name, log_stream_name)
    log_message = 'Lambda function invoked'
    
    cloudwatch.put_log_events(
        logGroupName=log_group_name,
        logStreamName=log_stream_name,
        logEvents=[
            {
                'timestamp': int(round(time.time() * 1000)),
                'message': log_message
            }
        ]
    )

    # Your existing code
    response = {
        'statusCode': 200,
        'headers': {
            'Content-Type': 'application/json',
        },
        'body': json.dumps(event)
    }

    # Introduce a delay of 500ms
    time.sleep(0.5)

    # End the X-Ray segment
    xray_recorder.end_segment()

    return response