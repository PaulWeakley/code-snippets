<?php
require_once 'mongodb_crud_client.php';
require_once 'mongodb_rest_client.php';

$uri = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);
$method = $_SERVER['REQUEST_METHOD'];
$uriSegments = explode('/', trim($uri, '/'));

$db_name = '';
$collection_name = '';
$id = '';

$config = [
];

if (count($uriSegments) >= 2) {
    $db_name = $uriSegments[0];
    $collection_name = $uriSegments[1];
}

if (count($uriSegments) === 2 && 'POST' === $method) {
    echo json_encode(["message" => "$db_name/$collection_name"]);
} else
if (count($uriSegments) === 3) {
    $id = $uriSegments[2];
    switch ($method) {
        case 'GET':
            $client = new MongoDB_REST_Client(new MongoDB_CRUD_Client($config));
            echo json_encode($client->read($db_name, $collection_name, $id));
            break;
        case 'PUT':
            echo json_encode(["message" => "$db_name/$collection_name/$id"]);
            break;
        case 'DELETE':
            echo json_encode(["message" => "$db_name/$collection_name/$id"]);
            break;
        default:
            header("HTTP/1.1 405 Method Not Allowed");
            echo json_encode(["message" => "Method not allowed"]);
            break;
    }
} else {
    header("HTTP/1.1 404 Not Found");
    echo json_encode(["message" => "Endpoint not found: " . $uri]);
}