import { Request, Response } from 'express';
import MongoDB_Config from '../services/MongoDB/CRUD/mongodb-config';
import MongoDB_Client_Builder from '../services/MongoDB/CRUD/mongodb-client-builder';
import MongoDB_CRUD_Client from '../services/MongoDB/CRUD/mongodb-crud-client';
import MongoDB_REST_Client from '../services/MongoDB/REST/mongodb-rest-client';
import config from '../config';
import {env} from '../config/env';
import { HealthResultEntry } from '../health/health-result-entry.model';
import { HealthResults } from '../health/health-results.model';

export class Health_Controller {
    constructor() {
        this.get = this.get.bind(this);
        const mongodb_config = new MongoDB_Config(
            env.MONGODB_USERNAME as string, 
            env.MONGODB_PASSWORD as string, 
            env.MONGODB_SERVER as string, 
            config.mongodb.retryWrites, 
            config.mongodb.w, 
            config.mongodb.appName
          );
          this.mongodbClientBuilder = new MongoDB_Client_Builder(mongodb_config);
    }

    private mongodbClientBuilder: MongoDB_Client_Builder;

    private createMongoDBClient(): MongoDB_REST_Client {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client(this.mongodbClientBuilder));
    }

    private async builHealthCheckMongoDB(mongodb_rest_client: MongoDB_REST_Client): Promise<HealthResultEntry> {
        const start = Date.now();
        try {
            await mongodb_rest_client.ping();
            return new HealthResultEntry('mongodb', true, Date.now() - start);
        } catch (e) {
            return new HealthResultEntry('mongodb', false, Date.now() - start, (e as Error).message);
        }
    }
    
    private async buildHealthCheckResponse(mongodb_rest_client: MongoDB_REST_Client): Promise<HealthResults> {
        const start = Date.now();
        const results = [await this.builHealthCheckMongoDB(mongodb_rest_client)]
        const duration = Date.now() - start;
        return new HealthResults(duration, results)
    }

    async get(req: Request, res: Response) {
        const client = this.createMongoDBClient();
        const response = await this.buildHealthCheckResponse(client);
        res.status(200).json(response);
    }
}