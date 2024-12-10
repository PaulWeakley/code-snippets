import autodynatrace
from src import create_app

import oneagent

if not oneagent.initialize():
    print('Error initializing OneAgent SDK.')

with oneagent.get_sdk().trace_incoming_remote_call('method', 'service', 'endpoint'):
    pass

app = create_app()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)