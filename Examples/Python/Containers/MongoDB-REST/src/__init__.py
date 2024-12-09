import configparser
import logging
import requests
import json
import os
from dotenv import load_dotenv
from flask import Flask

# OpenTelemetry imports
from opentelemetry import trace
from opentelemetry.instrumentation.flask import FlaskInstrumentor
from opentelemetry.instrumentation.requests import RequestsInstrumentor

from opentelemetry.sdk.resources import Resource
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.exporter.otlp.proto.http.trace_exporter import OTLPSpanExporter
from opentelemetry.sdk.trace.export import BatchSpanProcessor
from opentelemetry.sdk.metrics import MeterProvider
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
from opentelemetry.instrumentation.logging import LoggingInstrumentor
from opentelemetry.exporter.otlp.proto.http.metric_exporter import OTLPMetricExporter
from opentelemetry.metrics import set_meter_provider

from .services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from .services.MongoDB.CRUD.mongodb_crud_client import MongoDB_CRUD_Client
from .services.MongoDB.CRUD.mongodb_config import MongoDB_Config

from .controllers.health_controller import health_blueprint
from .controllers.mongodb_rest_controller import mongodb_rest_blueprint

app = Flask(__name__)

# Instrument Flask
FlaskInstrumentor().instrument_app(app)
RequestsInstrumentor().instrument()

# Load environment variables
config = configparser.ConfigParser()
config.read('config.ini')
load_dotenv()

mongodb_config = {
    'server': os.getenv('MONGODB_SERVER'),
    'username': os.getenv('MONGODB_USERNAME'),
    'password': os.getenv('MONGODB_PASSWORD'),
    'retryWrites': config.get('MongoDB', 'retryWrites'),
    'w': config.get('MongoDB', 'w'),
    'appName': config.get('MongoDB', 'appName')
}

trace_provider = TracerProvider()
trace_exporter = OTLPSpanExporter(endpoint=os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT"))
span_processor = BatchSpanProcessor(trace_exporter)
trace_provider.add_span_processor(span_processor)
"""
# Fetch OTLP endpoint and API key
OTLP_ENDPOINT = os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT")
OTLP_API_KEY = os.getenv("OTEL_EXPORTER_OTLP_API_KEY")
print(f"OTLP_ENDPOINT: {OTLP_ENDPOINT}")
print(f"OTLP_API_KEY: {OTLP_API_KEY}")

if not OTLP_ENDPOINT or not OTLP_API_KEY:
    raise ValueError("OTEL_EXPORTER_OTLP_ENDPOINT or OTEL_EXPORTER_OTLP_API_KEY is missing")

# Configure headers
headers = {"Authorization": f"Bearer {OTLP_API_KEY}"}

# Set up OpenTelemetry tracing
resource = Resource.create({"service.name": "flask-app"})
tracer_provider = TracerProvider(resource=resource)
trace.set_tracer_provider(tracer_provider)

span_processor = BatchSpanProcessor(
    OTLPSpanExporter(
        endpoint=f"{OTLP_ENDPOINT}/v1/traces",
        headers={"Authorization": f"Bearer {OTLP_API_KEY}"}
    )
)
tracer_provider.add_span_processor(span_processor)

# Set up OpenTelemetry metrics
metric_exporter = OTLPMetricExporter(
    endpoint=f"{OTLP_ENDPOINT}",
    headers={"Authorization": f"Bearer {OTLP_API_KEY}"}
)
metric_reader = PeriodicExportingMetricReader(metric_exporter)
meter_provider = MeterProvider(resource=resource, metric_readers=[metric_reader])"""

# Instrument logging for traces
LoggingInstrumentor().instrument(set_logging_format=True)

logging.basicConfig(level=logging.INFO)

# Example usage
'''
with tracer.start_as_current_span("example-span") as span:
    span.set_attribute("example-attribute", "value")
    print("Trace sent to Grafana Cloud!")

counter = meter.create_counter("example_counter", description="Example counter")
counter.add(1, {"example_label": "example_value"})
print("Metrics sent to Grafana Cloud!")
'''
# Set up the Loki handler
# Loki configuration
'''
LOKI_URL = os.getenv("LOKI_URL")
LOKI_API_KEY = os.getenv("LOKI_API_KEY")
class LokiHandler(logging.Handler):
    def emit(self, record):
        log_entry = self.format(record)
        headers = {
            "Authorization": f"Bearer {LOKI_API_KEY}",
            "Content-Type": "application/json",
        }
        payload = {
            "streams": [
                {
                    "stream": {
                        "application": "flask_app",
                        "environment": "production",
                    },
                    "values": [[str(int(record.created * 1e9)), log_entry]],
                }
            ]
        }
        try:
            response = requests.post(
                LOKI_URL, headers=headers, data=json.dumps(payload)
            )
            if response.status_code != 204:
                print(f"Failed to send log to Loki: {response.text}")
        except Exception as e:
            print(f"Error sending log to Loki: {e}")


# Configure the root logger
logger = logging.getLogger(__name__)
logger.setLevel(logging.INFO)

# Attach the custom Loki handler
loki_handler = LokiHandler()
formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
loki_handler.setFormatter(formatter)
logger.addHandler(loki_handler)
'''

@app.before_request
def before_request():
    print("Before request")

@app.after_request
def after_request(response):
    print("After request")
    return response

def create_app(mongodb_config: MongoDB_Config):
    app.url_map.strict_slashes = False
    app.mongodb_rest_client = MongoDB_REST_Client(mongodb_crud_client=MongoDB_CRUD_Client(mongodb_config))

    app.register_blueprint(health_blueprint, url_prefix='/api/health')
    app.register_blueprint(mongodb_rest_blueprint, url_prefix='/api/mongodb')

    return app
