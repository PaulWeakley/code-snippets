from flask import Blueprint, request, current_app

mongodb_rest_blueprint = Blueprint('mongodb_rest', __name__)

@mongodb_rest_blueprint.route('/<db_name>/<collection_name>/<id>', methods=['GET'])
def get(db_name, collection_name, id):
    response = current_app.mongodb_rest_client.get(db_name, collection_name, id)
    return response.body, response.status_code, { 'Content-Type': response.content_type }

@mongodb_rest_blueprint.route('/<db_name>/<collection_name>', methods=['POST'])
def post(db_name, collection_name):
    response = current_app.mongodb_rest_client.post(db_name, collection_name, request.get_json())
    return response.body, response.status_code, { 'Content-Type': response.content_type }

@mongodb_rest_blueprint.route('/<db_name>/<collection_name>/<id>', methods=['PUT', 'PATCH'])
def put(db_name, collection_name, id):
    response = current_app.mongodb_rest_client.put(db_name, collection_name, id, request.get_json())
    return response.body, response.status_code, { 'Content-Type': response.content_type }

@mongodb_rest_blueprint.route('/<db_name>/<collection_name>/<id>', methods=['DELETE'])
def delete(db_name, collection_name, id):
    response = current_app.mongodb_rest_client.delete(db_name, collection_name, id)
    return response.body, response.status_code, { 'Content-Type': response.content_type }