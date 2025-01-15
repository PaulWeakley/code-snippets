import requests
import time
import json

from concurrent.futures import ThreadPoolExecutor
import random
import string

def fetch_url(args):
    measure = time.time()
    response = requests.post(f'{args[0]}{time.time()}', json=args[1])
    return response, (time.time()-measure)*1000

def random_string(length=10):
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

def random_object(isChild=False):
    return {
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string(),
        random_string(): random_string() if isChild else random_object(True),
        random_string(): random_string() if isChild else random_object(True),
        random_string(): random_string() if isChild else random_object(True),
    }

base_urls = [
    
]
times = []
print('type,test_ms,start,container_time,ssl_handshake,deserialization,create_object,get_object,serialization,high_iops,delete_object,total')
for base_url in base_urls:
    endpoints = []
    for i in range(100):
        endpoints.append([f"https://{base_url}/api/telemetry/", random_object()])
    # Execute requests in parallel using ThreadPoolExecutor
    measure = time.time()
    with ThreadPoolExecutor(max_workers=25) as executor:
        results = executor.map(fetch_url, endpoints)
    times.append(f'{base_url}: {(time.time()-measure)*1000:.6f} ms')
    # Print results
    for response, milliseconds in results:
        data = response.json()
        print(f'{data["type"]},{milliseconds:.6f},{data["start"]:.6f},{data["container_time"]:.6f},{data["ssl_handshake"]:.6f},{data["deserialization"]:.6f},{data["create_object"]:.6f},{data["get_object"]:.6f},{data["serialization"]:.6f},{data["high_iops"]:.6f},{data["delete_object"]:.6f},{data["total"]:.6f}')

for time_log in times:
    print(time_log)
