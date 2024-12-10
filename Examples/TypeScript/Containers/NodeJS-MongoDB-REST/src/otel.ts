import { NodeSDK } from '@opentelemetry/sdk-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { diag, DiagConsoleLogger, DiagLogLevel } from '@opentelemetry/api';

// Enable diagnostic logging for debugging (optional)
diag.setLogger(new DiagConsoleLogger(), DiagLogLevel.INFO);

// Configure OTLP Trace Exporter
const traceExporter = new OTLPTraceExporter({
  url: 'http://grafana-alloy:4317',
});

// Create and configure the OpenTelemetry SDK
const sdk = new NodeSDK({
  traceExporter,
  instrumentations: [getNodeAutoInstrumentations()], // Auto instrumentations, including Express
});

// Initialize the SDK
export const startTelemetry = async (): Promise<void> => {
  try {
    sdk.start();
    console.log('OpenTelemetry initialized');
  } catch (error) {
    console.error('Error initializing OpenTelemetry', error);
  }

  // Gracefully shut down the SDK on process exit
  process.on('SIGTERM', async () => {
    try {
      await sdk.shutdown();
      console.log('OpenTelemetry SDK shut down');
    } catch (error) {
      console.error('Error shutting down OpenTelemetry SDK', error);
    } finally {
      process.exit(0);
    }
  });
};
