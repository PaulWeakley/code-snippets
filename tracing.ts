import { trace, ROOT_CONTEXT, defaultTextMapSetter } from "@opentelemetry/api";
import { NodeTracerProvider } from "@opentelemetry/sdk-trace-node";
import { BatchSpanProcessor } from "@opentelemetry/sdk-trace-base";
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { envDetector, Resource } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { W3CTraceContextPropagator } from "@opentelemetry/core";

interface TracingContext {
    tracer: ReturnType<typeof trace.getTracer>;
    api: typeof import("@opentelemetry/api").trace;
    propagator?: (span: ReturnType<typeof trace.getTracer>['startSpan']) => Record<string, string>;
}

export default async function initTracing(context: string, serviceName: string): Promise<TracingContext> {
    // Detect environment resources
    const detected = await envDetector.detect();
    const resources = new Resource({
        [ATTR_SERVICE_NAME]: serviceName,
    }).merge(detected);

    // Validate environment variables
    const tracingHost = process.env.TRACING_COLLECTOR_HOST || 'localhost';
    const tracingPort = process.env.TRACING_COLLECTOR_PORT || '4317';

    if (!tracingHost || !tracingPort) {
        throw new Error("Tracing collector host and port must be set.");
    }

    // Initialize tracer provider
    const provider = new NodeTracerProvider({
        resource: resources,
    });

    // Configure the OTLP exporter
    const exporter = new OTLPTraceExporter({
        url: `${tracingHost}:${tracingPort}`,
    });

    // Use BatchSpanProcessor for better performance in production
    const processor = new BatchSpanProcessor(exporter);
    provider.addSpanProcessor(processor);

    // Register the provider
    provider.register();

    // Create a header for propagation based on the context
    let createPropagationHeader: ((span: ReturnType<typeof trace.getTracer>['startSpan']) => Record<string, string>) | undefined;

    if (context === 'requester') {
        const propagator = new W3CTraceContextPropagator();
        createPropagationHeader = (createSpan) => {
            const span = createSpan("span-for-propagation");
            if (!span || !span.isRecording()) {
                throw new Error("Invalid or non-recording span provided for propagation.");
            }

            const carrier: Record<string, string> = {};
            propagator.inject(
                trace.setSpanContext(ROOT_CONTEXT, span.spanContext()),
                carrier,
                defaultTextMapSetter
            );
            return carrier;
        };
    }

    // Register instrumentations
    registerInstrumentations({
        instrumentations: [getNodeAutoInstrumentations()],
    });

    // Debugging: Log initialization details (optional)
    console.debug('Tracing initialized with service name:', serviceName);

    // Return tracing context
    return {
        tracer: trace.getTracer(serviceName),
        api: trace,
        propagator: createPropagationHeader,
    };
}