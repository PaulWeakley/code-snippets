import express, { Application, Request, Response } from 'express';

import healthRoutes from './routes/health.routes';
import mongodbRESTRoutes from './routes/mongodb.routes';

const app: Application = express();

app.use(express.json());
app.use('/api/health', healthRoutes); // add your routes
app.use('/api/mongodb', mongodbRESTRoutes); // add your routes

export default app;