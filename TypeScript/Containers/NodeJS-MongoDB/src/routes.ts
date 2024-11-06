// src/routes.ts
import { Router, Request, Response } from 'express';
import dotenv from 'dotenv';

import MongoDB_REST_Client from '../MongoDB/REST/src/MongDB_REST_Client';
import MongoDB_CRUD_Client from '../MongoDB/CRUD/src/MongoDB_CRUD_Client';
import MongoDB_Config from '../MongoDB/CRUD/src/MongoDB_Config';
import config from '../config';

dotenv.config();

const router: Router = Router();
const mongodb_config = new MongoDB_Config(
  process.env.MONGODB_USERNAME as string, 
  process.env.MONGODB_PASSWORD as string, 
  process.env.MONGODB_SERVER as string, 
  config.mongodb.retryWrites, 
  config.mongodb.w, 
  config.mongodb.appName
);

const createMongoDBClient = () => new MongoDB_REST_Client(new MongoDB_CRUD_Client(mongodb_config));

const buildHealthCheckMongoDB = async () => {
  const client = createMongoDBClient();
  try {
    await client.ping();
    return {
      isHealthy: true,
      responseTime: `${Math.round(process.hrtime()[1] / 1000000)} ms`
    };
  } catch (e) {
    return {
      isHealthy: false,
      responseTime: `${Math.round(process.hrtime()[1] / 1000000)} ms`,
      error: (e as Error).message
    };
  }
};

const buildHealthCheckResponse = async () => {
  const mongoHealth = await buildHealthCheckMongoDB();
  return {
    status: mongoHealth.isHealthy ? 'healthy' : 'unhealthy',
    details: {
      mongodb: mongoHealth
    },
    timestamp: new Date().toISOString(),
    version: '1.0.0'
  };
};

const handleHealthCheck = async (req: Request, res: Response) => {
  try {
    const healthCheckResponse = await buildHealthCheckResponse();
    res.json(healthCheckResponse);
  } catch (e) {
    res.status(500).send((e as Error).message);
  }
};
  
router.get('/health-check', handleHealthCheck);
router.post('/health-check', handleHealthCheck);

router.get('/:db_name/:collection_name/:id', async (req: Request, res: Response) => {
  const client = createMongoDBClient();
  const response = await client.get(req.params.db_name as string, req.params.collection_name as string, req.params.id as string);
  res.status(response.status_code).type(response.content_type).send(response.body);
});

router.post('/:db_name/:collection_name', async (req: Request, res: Response) => {
  const client = createMongoDBClient();
  const response = await client.post(req.params.db_name as string, req.params.collection_name as string, req.body);
  res.status(response.status_code).type(response.content_type).send(response.body);
});

const handlePutAndPatch = async (req: Request, res: Response) => {
  const client = createMongoDBClient();
  const response = await client.put(req.params.db_name as string, req.params.collection_name as string, req.params.id as string, req.body);
  res.status(response.status_code).type(response.content_type).send(response.body);
};

router.put('/:db_name/:collection_name/:id', handlePutAndPatch);
router.patch('/:db_name/:collection_name/:id', handlePutAndPatch);

router.delete('/:db_name/:collection_name/:id', async (req: Request, res: Response) => {
  const client = createMongoDBClient();
  const response = await client.delete(req.params.db_name, req.params.collection_name, req.params.id);
  res.status(response.status_code).type(response.content_type).send(response.body);
});

export default router;