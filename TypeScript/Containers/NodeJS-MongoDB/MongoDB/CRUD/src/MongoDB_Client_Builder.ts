import { MongoClient, ServerApiVersion, MongoClientOptions, Collection, ObjectId } from 'mongodb';
import IMongoDB_Config from './IMongoDB_Config';
import IMongoDB_Client_Builder from './IMongoDB_Client_Builder';

class MongoDB_Client_Builder implements IMongoDB_Client_Builder {
    private __config: IMongoDB_Config;

    constructor(config: IMongoDB_Config) {
        this.__config = config;
    }

    public get_client(): MongoClient {
        return new MongoClient(this.__config.toUri())
    }
}

export default MongoDB_Client_Builder;