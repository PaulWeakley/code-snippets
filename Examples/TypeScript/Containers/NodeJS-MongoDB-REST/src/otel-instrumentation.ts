import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-grpc';
import { PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { Resource } from '@opentelemetry/resources';

const traceExporter = new OTLPTraceExporter({
  url: 'grpc://0.0.0.0:4317', // Replace with your Grafana Alloy OTLP trace endpoint
  headers: {
    Authorization: `Bearer YOUR_API_KEY`, // Replace with your Grafana Alloy API key
  },
});

const metricExporter = new OTLPMetricExporter({
  url: 'grpc://0.0.0.0:4317', // Replace with your Grafana Alloy OTLP metric endpoint
  headers: {
    Authorization: `Bearer YOUR_API_KEY`, // Replace with your Grafana Alloy API key
  },
});

const sdk = new NodeSDK({
  traceExporter,
  metricReader: new PeriodicExportingMetricReader({
    exporter: metricExporter,
    exportIntervalMillis: 60000,
  }),
  instrumentations: [getNodeAutoInstrumentations()],
  resource: new Resource({
    'service.name': 'express-api', // Replace with your service name
  }),
});

sdk.start();
  //.then(() => console.log('OpenTelemetry SDK initialized for Express API'))
  //.catch((err) => console.error('Error initializing OpenTelemetry SDK', err));

// Graceful shutdown
process.on('SIGTERM', () => {
  sdk.shutdown()
    .then(() => console.log('OpenTelemetry SDK shut down'))
    .catch((err) => console.error('Error shutting down OpenTelemetry SDK', err))
    .finally(() => process.exit(0));
});
