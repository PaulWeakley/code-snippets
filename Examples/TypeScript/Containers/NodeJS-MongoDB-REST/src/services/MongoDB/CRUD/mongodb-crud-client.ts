import { MongoClient, ServerApiVersion, MongoClientOptions, Collection, ObjectId } from 'mongodb';
import IMongoDB_CRUD_Client_Builder from './imongodb-client-builder';

class MongoDB_CRUD_Client implements AsyncDisposable {
    private __builder: IMongoDB_CRUD_Client_Builder;
    private __client: MongoClient | null;

    constructor(builder: IMongoDB_CRUD_Client_Builder) {
        this.__builder = builder;
        this.__client = null;
    }
    
    async [Symbol.asyncDispose](): Promise<void> {
        await this.close();
    }

    private async __get_client(): Promise<MongoClient> {
        if (this.__client === null) {
            this.__client = this.__builder.get_client();
            await this.__client.connect();
        }
        return this.__client;
    }

    public async ping(): Promise<void> {
        const client = await this.__get_client();
        await client.db('admin').command({ ping: 1 });
    }

    public async close(): Promise<void> {
        if (this.__client !== null) {
            await this.__client.close();
            this.__client = null;
        }
    }

    private async __get_collection(db_name: string, collection_name: string): Promise<Collection> {
        const client = await this.__get_client();
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