import { Request, Response } from 'express';
import MongoDB_CRUD_Client from '../services/MongoDB/CRUD/mongodb-crud-client';

const db_name = 'test'
const collection_name = 'source'

export class Telemetry_Controller {
    constructor() {
        this.post = this.post.bind(this);
    }

    async post(req: Request, res: Response) {
        const start = Number(req.params.start);
        const container_time = Date.now() / 1000;
        const ssl_handshake = container_time - start;
        const start_ref = process.hrtime();
        
        let measure = process.hrtime();
        const payload = req.body;
        const deserialization = process.hrtime(measure);
        
        measure = process.hrtime();
        const client = new MongoDB_CRUD_Client();
        const document_id= await client.create(db_name, collection_name, payload);
        const create_object = process.hrtime(measure);

        measure = process.hrtime();
        const document = await client.read(db_name, collection_name, document_id);
        const get_object = process.hrtime(measure);
        
        measure = process.hrtime();
        const json_data = JSON.stringify(document);
        const serialization = process.hrtime(measure);

        measure = process.hrtime();
        for (let i = 0; i < 15; i++)
            await client.update(db_name, collection_name, document_id, { time: Date.now() });
        const high_iops = process.hrtime(measure);

        measure = process.hrtime();
        await client.delete(db_name, collection_name, document_id);
        const delete_object = process.hrtime(measure);

        const total = process.hrtime(start_ref);

        const telemetry = {
            start: start,
            container_time: container_time,
            ssl_handshake: ssl_handshake,
            deserialization: deserialization[0] * 1000 + deserialization[1] / 1e6,
            create_object: create_object[0] * 1000 + create_object[1] / 1e6,
            get_object: get_object[0] * 1000 + get_object[1] / 1e6,
            serialization: serialization[0] * 1000 + serialization[1] / 1e6,
            high_iops: high_iops[0] * 1000 + high_iops[1] / 1e6,
            delete_object: delete_object[0] * 1000 + delete_object[1] / 1e6,
            total: total[0] * 1000 + total[1] / 1e6,
            type: 'nodejs'
        };

        res.status(200).json(telemetry);
    }
}