import axios from 'axios';
import { Tracer, Span, SpanStatusCode } from '@opentelemetry/api';

interface TracingObj {
    api: {
        SpanStatusCode: typeof SpanStatusCode;
    };
    tracer: Tracer;
}

interface LogDetails {
    message: string;
    level: string;
    job: string;
    endpointLabel?: string;
    endpoint?: string;
    namespace: string;
}

const logging = (serviceName: string, context: string) => {
    return async (tracingObj: TracingObj) => {
        const { api, tracer } = tracingObj;

        const toLokiServer = async (details: LogDetails): Promise<boolean> => {
            const { message, level, job, endpointLabel, endpoint, namespace } = details;
            let error = false;
            let stream: Record<string, string> = {
                service_name: serviceName,
                level,
                job,
                namespace,
            };
            if (endpoint && endpointLabel) {
                stream[endpointLabel] = endpoint;
            }

            try {
                await axios.post(process.env.LOGS_TARGET!, {
                    streams: [
                        {
                            stream,
                            values: [
                                [`${Date.now() * 1000000}`, message]
                            ]
                        }
                    ]
                });
            } catch (err) {
                console.log(`Logging error: ${err}`);
                error = true;
            }

            return error;
        };

        const logEntry = async (details: LogDetails): Promise<void> => {
            let logSpan: Span | undefined;
            let error = false;
            if (context === 'requester') {
                logSpan = tracer.startSpan("log_to_loki");
            }

            if (process.env.LOGS_TARGET) {
                error = await toLokiServer(details);
            } else {
                console.log(details.message);
            }

            if (context === 'requester' && logSpan) {
                logSpan.setStatus({ code: (!error) ? api.SpanStatusCode.OK : api.SpanStatusCode.ERROR });
                logSpan.end();
            }
        };

        return logEntry;
    };
};

export default logging;