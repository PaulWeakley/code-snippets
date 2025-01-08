import { MongoClient, ServerApiVersion, MongoClientOptions, Collection, ObjectId } from 'mongodb';
import { env } from '../../../config/env';
import MongoDB_Config from './mongodb-config';

class MongoDB_CRUD_Client {
    private static __mongo_client: MongoClient;

    static {
        const mongodb_config = new MongoDB_Config(
            env.MONGODB_USERNAME,
            env.MONGODB_PASSWORD,
            env.MONGODB_SERVER,
            env.MONGODB_RETRY_WRITES === 'true',
            env.MONGODB_WRITE_CONCERN,
            env.MONGODB_APP_NAME,
            parseInt(env.MONGODB_MIN_POOL_SIZE),
            parseInt(env.MONGODB_MAX_POOL_SIZE),
            parseInt(env.MONGODB_WAIT_QUEUE_TIMEOUT_MS)
        );
        MongoDB_CRUD_Client.__mongo_client = new MongoClient(mongodb_config.toUri());
        MongoDB_CRUD_Client.__mongo_client.connect().then(() => {
            console.log('MongoDB connected');
        }).catch((error) => {
            console.error('MongoDB connection error:', error);
        });
    }

    public async ping(): Promise<void> {
        const client = MongoDB_CRUD_Client.__mongo_client;
        await client.db('admin').command({ ping: 1 });
    }

    private async __get_collection(db_name: string, collection_name: string): Promise<Collection> {
        const client = MongoDB_CRUD_Client.__mongo_client;
        return client.db(db_name).collection(collection_name);
    }

    public async create(db_name: string, collection_name: string, document: object): Promise<ObjectId> {
        const collection = await this.__get_collection(db_name, collection_name);
        const result = await collection.insertOne(document);
        return result.insertedId;
    }

    public async read(db_name: string, collection_name: string, document_id: ObjectId): Promise<object | null> {
        const collection = await this.__get_collection(db_name, collection_name);
        const document = await collection.findOne({ _id: document_id });
        return document;
    }

    public async update(db_name: string, collection_name: string, document_id: ObjectId, update_fields: object): Promise<object | null> {
        const collection = await this.__get_collection(db_name, collection_name);
        const result = await collection.findOneAndUpdate(
            { _id: document_id },
            { $set: update_fields },
            { returnDocument: 'after' }
        );
        return result ? result.value : null;
    }

    public async delete(db_name: string, collection_name: string, document_id: ObjectId): Promise<boolean> {
        const collection = await this.__get_collection(db_name, collection_name);
        const result = await collection.deleteOne({ _id: document_id });
        return result.deletedCount === 1;
    }
}

export default MongoDB_CRUD_Client;