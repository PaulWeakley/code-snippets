import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { Resource } from '@opentelemetry/resources';

class OpenTelemetrySDK {
  private static instance: NodeSDK | null = null;

  private constructor() {}

  public static getInstance(): NodeSDK {
    if (!OpenTelemetrySDK.instance) {
      OpenTelemetrySDK.instance = new NodeSDK({
        instrumentations: [getNodeAutoInstrumentations()],
        resource: new Resource({
          'service.name': 'express-api', // Replace with your service name
        }),
      });

      OpenTelemetrySDK.instance.start();
      console.log('OpenTelemetry SDK started');
    }

    return OpenTelemetrySDK.instance;
  }
}

export default OpenTelemetrySDK.getInstance();