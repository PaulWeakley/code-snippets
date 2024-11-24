// src/app.ts
import express, { Application, Request, Response } from 'express';
import healthRoutes from './routes/health.routes';
import mongodbRESTRoutes from './routes/mongodb.routes';

const app: Application = express();
const PORT = process.env.PORT || 3000;

app.use(express.json()); // for parsing application/json
app.use('/api/health', healthRoutes); // add your routes
app.use('/api/mongodb', mongodbRESTRoutes); // add your routes


export default app;
