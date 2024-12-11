<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Services\MongoDB\REST;
use App\Services\MongoDB\CRUD;

class MongoDB_REST_Controller extends Controller
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

    private function toResponseEntity($restResponse)
    {
        return response($restResponse->getBody(), $restResponse->getStatusCode())
            ->header('Content-Type', $restResponse->getContentType());
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request, $db_name, $collection_name)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->postAsync($db_name, $collection_name, $request->getContent()));
    }

    /**
     * Display the specified resource.
     */
    public function show(string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->getAsync($db_name, $collection_name, $id));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->putAsync($db_name, $collection_name, $id, $request->getContent()));
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->deleteAsync($db_name, $collection_name, $id));
    }
}
