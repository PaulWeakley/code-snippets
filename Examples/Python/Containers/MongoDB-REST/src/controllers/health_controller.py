import time
import os
from flask import Blueprint, jsonify, current_app

from src.services.MongoDB.REST.mongodb_rest_client import MongoDB_REST_Client
from src.health.health_result_entry import HealthResultEntry
from src.health.health_results import HealthResults

health_blueprint = Blueprint('health', __name__)

def build_health_check_mongodb(mongodb_rest_client: MongoDB_REST_Client) -> HealthResultEntry:
    start = time.time()
    try:
        mongodb_rest_client.ping()
        return HealthResultEntry('mongodb', True, round((time.time()-start) * 1000))
    except Exception as e:
        return HealthResultEntry('mongodb', False, round((time.time()-start) * 1000), str(e))
    

def build_health_check_response(mongodb_rest_client: MongoDB_REST_Client) -> HealthResults:
    start = time.time()
    results = [build_health_check_mongodb(mongodb_rest_client)]
    return HealthResults(round((time.time()-start) * 1000), results)

@health_blueprint.route('/', methods=['GET', 'POST'])
def health_check():
    try:
        return jsonify(build_health_check_response(current_app.mongodb_rest_client).to_dict())
    except Exception as e:
        return str(e), 500