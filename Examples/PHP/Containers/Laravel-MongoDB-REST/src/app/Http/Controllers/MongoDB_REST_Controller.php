<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Services\MongoDB\REST;
use App\Services\MongoDB\CRUD;

class MongoDB_REST_Controller extends Controller
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
        return $this->toResponseEntity($this->getMongoDBClient()->postAsync($db_name, $collection_name, $request->getContent()));
    }

    /**
     * Display the specified resource.
     */
    public function show(string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->getMongoDBClient()->getAsync($db_name, $collection_name, $id));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->getMongoDBClient()->putAsync($db_name, $collection_name, $id, $request->getContent()));
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->getMongoDBClient()->deleteAsync($db_name, $collection_name, $id));
    }
}
