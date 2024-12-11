<?php


namespace App\Services\MongoDB\CRUD;

class MongoDB_CRUD_Client implements IMongoDB_CRUD_Client
{
    protected IMongoDB_Client_Builder $mongoClientBuilder;
    protected ?\MongoDB\Client $mongoClient;

    public function __construct(IMongoDB_Client_Builder $mongoClientBuilder)
    {
        $this->mongoClientBuilder = $mongoClientBuilder;
        $this->mongoClient = null;
    }

    private function getClient(): \MongoDB\Client
    {
        if ($this->mongoClient === null) {
            $this->mongoClient = $this->mongoClientBuilder->build();
        }
        return $this->mongoClient;
    }

    public function ping(): bool
    {
        $this->getClient()->admin->command(['ping' => 1]);
        return true;
    }

    protected function getCollection($dbName, $collectionName)
    {
        return $this->getClient()->$dbName->$collectionName;
    }

    public function createAsync(string $dbName, string $collectionName, \MongoDB\Model\BSONDocument $document): string
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->insertOne($document);
        return $result->getInsertedId();
    }

    public function readAsync(string $dbName, string $collectionName, string $documentId): ?\MongoDB\Model\BSONDocument
    {
        $collection = $this->getCollection($dbName, $collectionName);
        return $collection->findOne(['_id' => new \MongoDB\BSON\ObjectId($documentId)]);
    }

    public function updateAsync(string $dbName, string $collectionName, string $documentId, \MongoDB\Model\BSONDocument $updateFields): ?\MongoDB\Model\BSONDocument
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $update = ['$set' => $updateFields];
        return $collection->findOneAndUpdate(
            ['_id' => new \MongoDB\BSON\ObjectId($documentId)],
            $update,
            ['returnDocument' => \MongoDB\Operation\FindOneAndUpdate::RETURN_DOCUMENT_AFTER]
        );
    }

    public function deleteAsync(string $dbName, string $collectionName, string $documentId): bool
    {
        $collection = $this->getCollection($dbName, $collectionName);
        $result = $collection->deleteOne(['_id' => new \MongoDB\BSON\ObjectId($documentId)]);
        return $result->getDeletedCount() == 1;
    }
}