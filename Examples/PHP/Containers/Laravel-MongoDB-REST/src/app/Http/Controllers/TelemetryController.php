<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Services\MongoDB\CRUD;
use MongoDB\Model\BSONDocument;

class TelemetryController extends Controller
{
    public function index(Request $request, $raw_start) { 
        $start = (float)$raw_start;
        $containerTime = microtime(true);
        $sslHandshake = $containerTime - $start;
        $start_ref = microtime(true);

        $measure = microtime(true);
        $data = $request->getContent();
        $body = new BSONDocument(json_decode($data, true));
        $deserialization = microtime(true) - $measure;

        $db_name = "test";
        $collection_name = "source";
        $measure = microtime(true);
        $client = new CRUD\MongoDB_CRUD_Client();
        $document_id = $client->createAsync($db_name, $collection_name, $body);
        $createObject = microtime(true) - $measure;

        $measure = microtime(true);
        $document = $client->readAsync($db_name, $collection_name, $document_id);
        $getObject = microtime(true) - $measure;

        $measure = microtime(true);
        $jsonData = json_encode($document);
        $serialization = microtime(true) - $measure;

        $measure = microtime(true);
        for ($i = 0; $i < 15; $i++) {
            $json_update = '{"time": "' . date('Y-m-d H:i:s') . '"}';
            $client->updateAsync($db_name, $collection_name, $document_id, new BSONDocument(json_decode($json_update, true)));
        }
        $highIOps = microtime(true) - $measure;

        $measure = microtime(true);
        $client->deleteAsync($db_name, $collection_name, $document_id);
        $deleteObject = microtime(true) - $measure;

        $total = microtime(true) - $start_ref;

        $telemetry = [
            'start' => $raw_start,
            'container_time' => $containerTime,
            'ssl_handshake' => $sslHandshake,
            'deserialization' => $deserialization * 1000,
            'create_object' => $createObject * 1000,
            'get_object' => $getObject * 1000,
            'serialization' => $serialization * 1000,
            'high_iops' => $highIOps * 1000,
            'delete_object' => $deleteObject * 1000,
            'total' => $total * 1000
        ];

        return response()->json($telemetry, 200);
    }
}