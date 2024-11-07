import { MongoClient } from "mongodb";

interface IMongoDB_Client_Builder {
    get_client(): MongoClient;
}

export default IMongoDB_Client_Builder;