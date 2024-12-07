<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Services\MongoDB\REST;
use App\Services\MongoDB\CRUD;

class MongoDB_REST_Controller extends Controller
{
    public function __construct()
    {
        // Initialize MongoDB client
        
    }
    private function createMongoDBClient()
    {
        // Assuming MongoDB_REST_Client and MongoDB_CRUD_Client are defined elsewhere
        return new REST\MongoDB_REST_Client(new CRUD\MongoDB_CRUD_Client(new CRUD\MongoDB_Client_Builder("")));
    }

    private function toResponseEntity($response)
    {
        return $response->then(function ($restResponse) {
            return response($restResponse->getBody(), $restResponse->getStatusCode())
                ->header('Content-Type', $restResponse->getContentType());
        })->catch(function ($ex) {
            return response("An error occurred: " . $ex->getMessage(), 500);
        });
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request, $db_name, $collection_name)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->postAsync($db_name, $collection_name, $request->Body));
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
        return $this->toResponseEntity($this->createMongoDBClient()->putAsync($db_name, $collection_name, $id, $request->Body));
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(string $db_name, string $collection_name, string $id)
    {
        return $this->toResponseEntity($this->createMongoDBClient()->deleteAsync($db_name, $collection_name, $id));
    }
}
