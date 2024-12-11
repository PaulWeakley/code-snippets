<?php

return [
    'server' => env('MONGODB_SERVER', ''),
    'username' => env('MONGODB_USERNAME', ''),
    'password' => env('MONGODB_PASSWORD', ''),
    'appName' => env('MONGODB_APPNAME', ''),
    'retryWrites' => env('MONGODB_RETRYWRITES', "true"),
    'w' => env('MONGODB_W', 'majority'),
];