import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { diag, DiagConsoleLogger, DiagLogLevel } from '@opentelemetry/api';

// Set up OpenTelemetry diagnostics (optional)
diag.setLogger(new DiagConsoleLogger(), DiagLogLevel.INFO);

// Configure the OTLP Trace Exporter
const traceExporter = new OTLPTraceExporter({
  url: process.env.OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4317', // Replace with your OTLP endpoint
});

// Create and configure the NodeSDK
const sdk = new NodeSDK({
  traceExporter,
  instrumentations: [getNodeAutoInstrumentations()],
});

// Initialize the OpenTelemetry SDK
sdk.start()
/*  .then(() => {
    console.log('OpenTelemetry auto-instrumentation initialized.');
  })
  .catch((error) => {
    console.error('Error initializing OpenTelemetry:', error);
  });*/

// Ensure proper shutdown on process termination
process.on('SIGTERM', async () => {
  try {
    await sdk.shutdown();
    console.log('OpenTelemetry shut down successfully.');
  } catch (err) {
    console.error('Error shutting down OpenTelemetry:', err);
  }
});
