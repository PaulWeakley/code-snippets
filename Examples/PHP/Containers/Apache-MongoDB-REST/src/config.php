<?php
require 'vendor/autoload.php';

use MongoDB\Client;

function getDatabase() {
    $client = new Client("mongodb://localhost:27017");
    return $client->selectDatabase('yourDatabaseName');
}
