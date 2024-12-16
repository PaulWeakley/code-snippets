from ddtrace.profiling import Profiler
prof = Profiler()
prof.start() 
from datadog import initialize, statsd
options = {
    'statsd_host':'127.0.0.1',
    'statsd_port':8125
}
initialize(**options)

from src import create_app

app = create_app()

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)