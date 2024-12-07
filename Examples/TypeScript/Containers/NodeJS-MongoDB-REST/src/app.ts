// src/app.ts
import express, { Application, Request, Response } from 'express';
import promClient from 'prom-client';
import Pyroscope from '@pyroscope/nodejs';
import expressMiddleware from '@pyroscope/nodejs';


import healthRoutes from './routes/health.routes';
import mongodbRESTRoutes from './routes/mongodb.routes';

// Prometheus client registration
const app: Application = express();
// Prometheus metrics setup
const register = promClient.register;
register.setContentType("text/plain; version=0.0.4; charset=utf-8");
//promClient.collectDefaultMetrics({});

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

app.use(express.json()); // for parsing application/json
// Metrics endpoint handler (for Prometheus scraping)
app.get('/metrics', async (req: Request, res: Response) => {
    res.set('Content-Type', register.contentType);
    res.send(await register.metrics());
});
app.use('/api/health', healthRoutes); // add your routes
app.use('/api/mongodb', mongodbRESTRoutes); // add your routes


export default app;
