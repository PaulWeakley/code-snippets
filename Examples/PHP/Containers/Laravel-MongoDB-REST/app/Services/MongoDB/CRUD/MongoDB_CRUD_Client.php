<?php

namespace App\Services\MongoDB\CRUD;

class MongoDB_CRUD_Client implements IMongoDB_CRUD_Client
{
    protected $mongoClient;

    public function __construct($mongoClient)
    {
        $this->mongoClient = $mongoClient;
    }

    public function ping(): bool
    {
        $this->mongoClient->admin->command(['ping' => 1]);
        return true;
    }

    protected function getCollection($dbName, $collectionName)
    {
        return $this->mongoClient->$dbName->$collectionName;
    }

    public function createAsync(string $dbName, string $collectionName, \MongoDB\Model\BSONDocument $document): string
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->insertOne($document);
        return $result->getInsertedId();
    }

    public function readAsync(string $dbName, string $collectionName, string $documentId): \MongoDB\Model\BSONDocument
    {
        $collection = $this->getCollection($dbName, $collectionName);
        return $collection->findOne(['_id' => $documentId]);
    }

    public function updateAsync(string $dbName, string $collectionName, string $documentId, \MongoDB\Model\BSONDocument $updateFields): ?\MongoDB\Model\BSONDocument
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $update = ['$set' => $updateFields];
        return $collection->findOneAndUpdate(
            ['_id' => $documentId],
            $update,
            ['returnDocument' => \MongoDB\Operation\FindOneAndUpdate::RETURN_DOCUMENT_AFTER]
        );
    }

    public function deleteAsync(string $dbName, string $collectionName, string $documentId): bool
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->deleteOne(['_id' => $documentId]);
        return $result->getDeletedCount() == 1;
    }
}