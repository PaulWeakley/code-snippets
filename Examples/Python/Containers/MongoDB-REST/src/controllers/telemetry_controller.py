import time
import json
from flask import Blueprint, request, current_app, jsonify

telemetry_blueprint = Blueprint('telemetry', __name__)

db_name = 'test'
collection_name = 'source'

@telemetry_blueprint.route('/<int:start>', methods=['POST'])
def post(start):
    container_time = time.time()
    ssl_handshake = container_time - start
    start_ref = time.time()

    measure = time.time()
    data = request.json
    deserialization = time.time() - measure

    measure = time.time()
    client = current_app.mongodb_rest_client.get_client()
    document_id = client.create(db_name, collection_name, data)
    create_object = time.time() - measure

    measure = time.time()
    document = client.read(db_name, collection_name, document_id)
    get_object = time.time() - measure

    measure = time.time()
    json_data = json.dumps(document, default=str)
    serialization = time.time() - measure

    measure = time.time()
    for _ in range(15):
        client.update(db_name, collection_name, document_id, {'time': time.time()})
    high_iops = time.time() - measure

    measure = time.time()
    client.delete(db_name, collection_name, document_id)
    delete_object = time.time() - measure

    total = time.time() - start_ref

    telemetry = {
        'start': start,
        'container_time': container_time,
        'ssl_handshake': ssl_handshake,
        'deserialization': deserialization * 1000,
        'create_object': create_object * 1000,
        'get_object': get_object * 1000,
        'serialization': serialization * 1000,
        'high_iops': high_iops * 1000,
        'delete_object': delete_object * 1000,
        'total': total * 1000,
        'type': 'python'
    }

    return jsonify(telemetry), 200
