import { Request, Response } from 'express';
import MongoDB_Config from '../services/MongoDB/CRUD/mongodb-config';
import MongoDB_Client_Builder from '../services/MongoDB/CRUD/mongodb-client-builder';
import MongoDB_CRUD_Client from '../services/MongoDB/CRUD/mongodb-crud-client';
import MongoDB_REST_Client from '../services/MongoDB/REST/mongodb-rest-client';
import config from '../config';
import {env} from '../config/env';

export class MongoDB_REST_Controller {
    constructor() {
        this.get = this.get.bind(this);
        this.post = this.post.bind(this);
        this.put = this.put.bind(this);
        this.delete = this.delete.bind(this);
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

    async get(req: Request, res: Response) {
        const client = this.createMongoDBClient();
        const response = await client.get(req.params.db_name as string, req.params.collection_name as string, req.params.id as string);
        res.status(response.status_code).type(response.content_type).send(response.body);
    }

    async post(req: Request, res: Response) {
        const client = this.createMongoDBClient();
        const response = await client.post(req.params.db_name, req.params.collection_name, req.body);
        res.status(response.status_code).type(response.content_type).send(response.body)
    }

    async put(req: Request, res: Response) {
        const client = this.createMongoDBClient();
        const response = await client.put(req.params.db_name as string, req.params.collection_name as string, req.params.id as string, req.body);
        res.status(response.status_code).type(response.content_type).send(response.body);
    }

    async delete(req: Request, res: Response) {
        const client = this.createMongoDBClient();
        const response = await client.delete(req.params.db_name, req.params.collection_name, req.params.id);
        res.status(response.status_code).type(response.content_type).send(response.body)
    }
}