<?php

namespace App\Services\MongoDB\CRUD;

interface IMongoDB_CRUD_Client
{
    public function ping(): bool;
    public function createAsync(string $dbName, string $collectionName, \MongoDB\Model\BSONDocument $document): string;
    public function readAsync(string $dbName, string $collectionName, string $documentId): \MongoDB\Model\BSONDocument;
    public function updateAsync(string $dbName, string $collectionName, string $documentId, \MongoDB\Model\BSONDocument $updateFields): ?\MongoDB\Model\BSONDocument;
    public function deleteAsync(string $dbName, string $collectionName, string $documentId): bool;
}