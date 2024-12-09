import time
import os
from flask import Blueprint, jsonify, current_app

from opentelemetry.metrics import get_meter_provider, MeterProvider
from opentelemetry.sdk.metrics import Counter, MeterProvider as SdkMeterProvider
from opentelemetry.exporter.otlp.proto.grpc.metric_exporter import OTLPMetricExporter
from opentelemetry.sdk.metrics.export import PeriodicExportingMetricReader
from opentelemetry.metrics import set_meter_provider

from src.services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from src.health.health_result_entry import HealthResultEntry
from src.health.health_results import HealthResults

# Configure OpenTelemetry Metrics
exporter = OTLPMetricExporter(endpoint=os.getenv("OTEL_EXPORTER_OTLP_ENDPOINT"))
metric_reader = PeriodicExportingMetricReader(exporter)
meter_provider = SdkMeterProvider(metric_readers=[metric_reader])
set_meter_provider(meter_provider)

# Create a meter and a custom counter
meter = get_meter_provider().get_meter("custom-metrics")
request_counter = meter.create_counter(
    name="request_count",
    description="Counts the number of requests to the application",
    unit="1",
)

health_blueprint = Blueprint('health', __name__)

def build_health_check_mongodb(mongodb_rest_client: MongoDB_REST_Client) -> HealthResultEntry:
    start = time.time()
    try:
        mongodb_rest_client.ping()
        return HealthResultEntry('mongodb', True, round((time.time()-start) * 1000))
    except Exception as e:
        return HealthResultEntry('mongodb', False, round((time.time()-start) * 1000), str(e))
    

def build_health_check_response(mongodb_rest_client: MongoDB_REST_Client) -> HealthResults:
    start = time.time()
    results = [build_health_check_mongodb(mongodb_rest_client)]
    return HealthResults(round((time.time()-start) * 1000), results)

@health_blueprint.route('/', methods=['GET', 'POST'])
def health_check():
    try:
        request_counter.add(1, {"endpoint": "/health"}) 
        return jsonify(build_health_check_response(current_app.mongodb_rest_client).to_dict())
    except Exception as e:
        return str(e), 500