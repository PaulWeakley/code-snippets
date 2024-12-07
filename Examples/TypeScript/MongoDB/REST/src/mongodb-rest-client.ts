import { ObjectId } from 'mongodb';
import MongoDB_CRUD_Client from '../CRUD/mongodb-crud-client';
import REST_Response from './rest-response';


class MongoDB_REST_Client {
    private __crud_client: MongoDB_CRUD_Client;

    constructor(mongodb_crud_client: MongoDB_CRUD_Client) {
        this.__crud_client = mongodb_crud_client;
    }

    private __bad_request(message: string): REST_Response {
        return new REST_Response(400, 'text/plain', message);
    }

    private __error(e: Error): REST_Response {
        return this.__error_message(e.message);
    }

    private __error_message(message: string): REST_Response {
        return new REST_Response(500, 'text/plain', message);
    }

    private __not_found(id: ObjectId): REST_Response {
        return new REST_Response(404, 'text/plain', `Error: Document with id ${id} not found`);
    }

    private __ok(data: any): REST_Response {
        return new REST_Response(200, 'application/json', JSON.stringify(data));
    }

    private __created(id: ObjectId): REST_Response {
        return new REST_Response(201, 'text/plain', id.toString());
    }

    private __deleted(id: string): REST_Response {
        return new REST_Response(200, 'text/plain', `Document with id ${id} deleted`);
    }

    private __verify_parameters(db_name: string | null, collection_name: string | null, id: string | null, is_id_required: boolean, data: any | null, is_data_required: boolean): REST_Response | null {
        if (!db_name) {
            return this.__bad_request('Error: Missing db_name parameter');
        }
        if (!collection_name) {
            return this.__bad_request('Error: Missing collection_name parameter');
        }
        if (is_id_required) {
            if (!id) {
                return this.__bad_request('Error: Missing id parameter');
            } else if (!ObjectId.isValid(id)) {
                return this.__bad_request('Error: Invalid id');
            }
        }
        if (is_data_required && !data) {
            return this.__bad_request('Error: Missing body');
        }
        return null;
    }

    public async ping(): Promise<void> {
        await this.__crud_client.ping();
    }

    public async get(db_name: string | null, collection_name: string | null, id: string | null): Promise<REST_Response> {
        try {
            const error = this.__verify_parameters(db_name, collection_name, id, true, null, false);
            if (error) {
                return error;
            }
            var object_id = new ObjectId(id!);
            const document = await this.__crud_client.read(db_name!, collection_name!, object_id);
            if (document) {
                return this.__ok(document);
            }
            return this.__not_found(object_id);
        } catch (e) {
            return this.__error(e as Error);
        }
    }

    public async post(db_name: string | null, collection_name: string | null, data: any | null): Promise<REST_Response> {
        try {
            const error = this.__verify_parameters(db_name, collection_name, null, false, data, true);
            if (error) {
                return error;
            }

            const document_id = await this.__crud_client.create(db_name!, collection_name!, data);
            if (document_id) {
                return this.__created(document_id);
            }
            return this.__error_message('Failed to create document');
        } catch (e) {
            return this.__error(e as Error);
        }
    }

    public async put(db_name: string | null, collection_name: string | null, id: string | null, data: any | null): Promise<REST_Response> {
        try {
            const error = this.__verify_parameters(db_name, collection_name, id, true, data, true);
            if (error) {
                return error;
            }
            var object_id = new ObjectId(id!);
            const document = await this.__crud_client.update(db_name!, collection_name!, object_id, data);
            if (document) {
                return this.__ok(document);
            }
            return this.__not_found(object_id);
        } catch (e) {
            return this.__error(e as Error);
        }
    }

    public async delete(db_name: string | null, collection_name: string | null, id: string | null): Promise<REST_Response> {
        try {
            const error = this.__verify_parameters(db_name, collection_name, id, true, null, false);
            if (error) {
                return error;
            }

            var object_id = new ObjectId(id!);
            const result = await this.__crud_client.delete(db_name!, collection_name!, object_id);
            if (result) {
                return this.__deleted(id!);
            }
            return this.__not_found(object_id);
        } catch (e) {
            return this.__error(e as Error);
        }
    }
}

export default MongoDB_REST_Client;

