import json
from interop_serializer import InteropSerializer
from datetime import datetime
import uuid

def __main__():
    # Specify the path to the JSON file
    resources_location = './../../Resources/Interop/'
    file_path = resources_location+'interop_example.json'

    # Read the JSON file
    with open(file_path, 'r') as file:
        json_data = json.load(file)

    root = InteropSerializer.deserialize(json_data)
    serialized = InteropSerializer.serialize(root)
    
    x = json.dumps(json_data)
    y = json.dumps(serialized)
    if x == y:
        print("The single serialization and deserialization are correct")
    else:
        print("The single serialization and deserialization are incorrect")

    file_path = resources_location+'json_list.json'
    with open(file_path, 'r') as file:
        json_data = json.load(file)

    #print(json.dumps(json_data))
    list_entities = InteropSerializer.serialize(json_data)
    #print(json.dumps(list_entities))
    list = InteropSerializer.deserialize(list_entities)
    #print(list)

    x = json.dumps(json_data)
    y = json.dumps(list)
    if x == y:
        print("The list serialization and deserialization are correct")
    else:
        print("The list serialization and deserialization are incorrect")
    

def to_cloud_event(spec_version, data_type, source, subject, data):
    event_id = str(uuid.uuid4())
    date_time = datetime.now(datetime.timezone.utc).isoformat() + "Z"
    return {
        "specversion" : str(spec_version),
        "type" : str(data_type),
        "source" : str(source),
        "subject" : str(subject),
        "id" : event_id,
        "time" : date_time,
        "datacontenttype" : "text/interop",
        "data" : InteropSerializer.serialize(data)
    }


__main__()