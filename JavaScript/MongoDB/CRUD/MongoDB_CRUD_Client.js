import { MongoClient, ServerApiVersion } from 'mongodb';

class MongoDB_CRUD_Client {
    constructor(config) {
        this.config = config;
        this.uri = `mongodb+srv://${config.username}:${config.password}@${config.server}/?retryWrites=${config.retryWrites}&w=${config.w}&appName=${config.appName}`;
        this.client = null;
    }

    async __get_client() {
        if (this.client === null) {
            this.client = new MongoClient(this.uri, { serverApi: ServerApiVersion.v1 });
            await this.client.connect();
        }
        return this.client;
    }

    async get_collection(db_name, collection_name) {
        const client = await this.__get_client();
        return client.db(db_name).collection(collection_name);
    }

    async create(db_name, collection_name, document) {
        const collection = await this.get_collection(db_name, collection_name);
        const result = await collection.insertOne(document);
        return result.insertedId;
    }

    async read(db_name, collection_name, document_id) {
        const collection = await this.get_collection(db_name, collection_name);
        const document = await collection.findOne({ _id: document_id });
        return document;
    }

    async update(db_name, collection_name, document_id, update_fields) {
        const collection = await this.get_collection(db_name, collection_name);
        const result = await collection.findOneAndUpdate(
            { _id: document_id },
            { $set: update_fields },
            { returnDocument: 'after' }
        );
        return result.value;
    }

    async delete(db_name, collection_name, document_id) {
        const collection = await this.get_collection(db_name, collection_name);
        const result = await collection.deleteOne({ _id: document_id });
        return result.deletedCount === 1;
    }

    async ping() {
        const client = await this.__get_client();
        await client.db('admin').command({ ping: 1 });
    }

    async close() {
        if (this.client !== null) {
            await this.client.close();
            this.client = null;
        }
    }
}

export default MongoDB_CRUD_Client;