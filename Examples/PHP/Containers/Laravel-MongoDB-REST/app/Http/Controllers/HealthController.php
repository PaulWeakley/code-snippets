<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Services\MongoDB\REST;
use App\Services\MongoDB\CRUD;
use App\Models\HealthResultEntry;
use App\Models\HealthResults;
use Exception;

class HealthController extends Controller
{
    private static $mongoDBClientBuilder;
    private static function getMongoDBClientBuilder(): CRUD\MongoDB_Client_Builder
    {
        if (is_null(self::$mongoDBClientBuilder)) {
            self::$mongoDBClientBuilder = new CRUD\MongoDB_Client_Builder(
                new CRUD\MongoDB_Config(
                    config('mongodb.username'),
                    config('mongodb.password'),
                    config('mongodb.server'),
                    config('mongodb.retryWrites'),
                    config('mongodb.w'),
                    config('mongodb.appName')
                )
            );
        }
        return self::$mongoDBClientBuilder;
    }

    private function createMongoDBClient()
    {
        // Assuming MongoDB_REST_Client and MongoDB_CRUD_Client are defined elsewhere
        return new REST\MongoDB_REST_Client(new CRUD\MongoDB_CRUD_Client(self::getMongoDBClientBuilder()));
    }

    public function index() { return $this->healthCheck(); }

    public function store() { return $this->healthCheck(); }

    public function buildHealthCheckMongoDB($mongodbRestClient)
    {
        $start = microtime(true);
        try {
            $mongodbRestClient->ping();
            return new HealthResultEntry('mongodb', true, round((microtime(true) - $start) * 1000));
        } catch (Exception $e) {
            return new HealthResultEntry('mongodb', false, round((microtime(true) - $start) * 1000), $e->getMessage());
        }
    }

    public function buildHealthCheckResponse($mongodbRestClient)
    {
        $start = microtime(true);
        $results = [$this->buildHealthCheckMongoDB($mongodbRestClient)];
        return new HealthResults(round((microtime(true) - $start) * 1000), $results);
    }

    public function healthCheck()
    {
        try {
            $healthCheckResponse = $this->buildHealthCheckResponse($this->createMongoDBClient());
            return response()->json($healthCheckResponse->jsonSerialize());
        } catch (Exception $e) {
            return response($e->getMessage(), 500);
        }
    }
}