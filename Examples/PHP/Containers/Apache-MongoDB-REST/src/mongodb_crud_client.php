<?php

use MongoDB\Client;
use MongoDB\BSON\ObjectId;

require_once 'vendor/autoload.php'; // Make sure to include the MongoDB library

class MongoDB_CRUD_Client {
    private $config;
    private $uri;
    private $client = null;

    public function __construct($config) {
        $this->config = $config;
        $this->uri = sprintf(
            "mongodb+srv://%s:%s@%s/?retryWrites=%s&w=%s&appName=%s",
            $config['username'],
            $config['password'],
            $config['server'],
            $config['retryWrites'],
            $config['w'],
            $config['appName']
        );
    }

    private function getClient() {
        if ($this->client === null) {
            $this->client = new Client($this->uri);
        }
        return $this->client;
    }

    public function ping() {
        $this->getClient()->admin->command(['ping' => 1]);
    }

    public function close() {
        $this->client = null; // MongoDB\Client does not have a close method
    }

    public function getCollection($dbName, $collectionName) {
        $client = $this->getClient();
        return $client->$dbName->$collectionName;
    }

    public function create($dbName, $collectionName, $document) {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->insertOne($document);
        return $result->getInsertedId();
    }

    public function read($dbName, $collectionName, $documentId) {
        $collection = $this->getCollection($dbName, $collectionName);
        $document = $collection->findOne(['_id' => new ObjectId($documentId)]);
        return $document;
    }

    public function update($dbName, $collectionName, $documentId, $updateFields) {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->findOneAndUpdate(
            ['_id' => new ObjectId($documentId)],
            ['$set' => $updateFields],
            ['returnDocument' => MongoDB\Operation\FindOneAndUpdate::RETURN_DOCUMENT_AFTER]
        );
        return $result;
    }

    public function delete($dbName, $collectionName, $documentId) {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->deleteOne(['_id' => new ObjectId($documentId)]);
        return $result->getDeletedCount() == 1;
    }
}
?>