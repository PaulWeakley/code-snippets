<?php

return [
    'server' => env('MONGODB_SERVER', ''),
    'username' => env('MONGODB_USERNAME', ''),
    'password' => env('MONGODB_PASSWORD', ''),
    'appName' => env('MONGODB_APPNAME', ''),
    'retryWrites' => env('MONGODB_RETRYWRITES', "true"),
    'writeConcern' => env('MONGODB_WRITE_CONCERN', 'majority'),
    'minPoolSize' => env('MONGODB_MIN_POOL_SIZE', 10),
    'maxPoolSize' => env('MONGODB_MAX_POOL_SIZE', 50),
    'waitQueueTimeoutMS' => env('MONGODB_WAIT_QUEUE_TIMEOUT_MS', 1000),
];