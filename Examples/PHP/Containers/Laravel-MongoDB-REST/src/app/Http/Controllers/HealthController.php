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
    private $mongoDbRESTClient;

    public function __construct()
    {
        $this->mongoDbRESTClient = new REST\MongoDB_REST_Client(new CRUD\MongoDB_CRUD_Client());
    }

    private function getMongoDBClient(): REST\MongoDB_REST_Client
    {
        return $this->mongoDbRESTClient;
    }

    public function index() { return $this->healthCheck(); }

    public function store() { return $this->healthCheck(); }

    public function buildHealthCheckMongoDB($mongodbRestClient)
    {
        $start = microtime(true);
        try {
            $mongodbRestClient->ping();
            return new HealthResultEntry('mongodb', round((microtime(true) - $start) * 1000), 'healthy');
        } catch (Exception $e) {
            return new HealthResultEntry('mongodb', round((microtime(true) - $start) * 1000), 'unhealthy', $e->getMessage());
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
            $healthCheckResponse = $this->buildHealthCheckResponse($this->getMongoDBClient());
            return response()->json($healthCheckResponse->jsonSerialize(), $healthCheckResponse->isHealthy() ? 200 : 500);
        } catch (Exception $e) {
            return response($e->getMessage(), 500);
        }
    }
}