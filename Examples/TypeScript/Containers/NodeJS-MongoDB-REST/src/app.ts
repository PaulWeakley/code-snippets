import { startTelemetry } from './otel';
import express, { Application, Request, Response } from 'express';
import promClient from 'prom-client';
//import Pyroscope from '@pyroscope/nodejs'

import healthRoutes from './routes/health.routes';
import mongodbRESTRoutes from './routes/mongodb.routes';

// Initialize OpenTelemetry SDK
startTelemetry();

/*Pyroscope.init({
    appName: 'nodejs-typescript-mongodb-rest-api', // Replace with your application name
    serverAddress: process.env.PYROSCOPE_SERVER_URL || 'http://pyroscope:4040', // Pyroscope server address
  });*/

// Prometheus client registration
const app: Application = express();

const register = promClient.register;
register.setContentType('text/plain; version=0.0.4; charset=utf-8');
promClient.collectDefaultMetrics();

app.get('/metrics', async (req: Request, res: Response) => {
  res.set('Content-Type', register.contentType);
  res.send(await register.metrics());
});

// Initialise the Pyroscope library to send pprof data.
// Initialize the Pyroscope library to send profiling data
/* Pyroscope.init({
    serverAddress: process.env.PYROSCOPE_SERVER || "http://localhost:4040", // Pyroscope server address
    appName: "nodejs-typescript-mongodb-rest-api", // Set an appropriate app name
    tags: {
        instance: "server",
        environment: process.env.NODE_ENV || "development",
    }
}); */

// Apply Pyroscope Express middleware
//app.use(expressMiddleware.expressMiddleware());

app.use(express.json());
app.use('/api/health', healthRoutes); // add your routes
app.use('/api/mongodb', mongodbRESTRoutes); // add your routes


export default app;
