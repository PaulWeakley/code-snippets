<?php

namespace App\Services\MongoDB\REST;

use function MongoDB\BSON\fromJSON;
use MongoDB\Model\BSONDocument;
use App\Services\MongoDB\CRUD\IMongoDB_CRUD_Client;
use App\Services\MongoDB\REST\REST_Response;

class MongoDB_REST_Client
{
    private $crud_client;

    public function __construct(IMongoDB_CRUD_Client $mongodb_crud_client)
    {
        $this->crud_client = $mongodb_crud_client;
    }

    /*private static function jsonWriterSettings()
    {
        return ['outputMode' => JsonOutputMode::RELAXED_EXTENDED_JSON];
    }*/

    private static function badRequest($message)
    {
        return new REST_Response(400, "text/plain", $message);
    }

    private static function errorMessage($e)
    {
        return self::errorMessageFromString($e->getMessage());
    }

    private static function errorMessageFromString($message)
    {
        return new REST_Response(500, "text/plain", $message);
    }

    private static function notFound($id)
    {
        return new REST_Response(404, "text/plain", "Exception: Document with id $id not found");
    }

    private static function ok($data)
    {
        $data->_id = (string)$data->_id;
        /*if (isset($data['_id']) && $data['_id'] instanceof ObjectId) {
            $data['_id'] = (string) $data['_id'];
        }*/
        return new REST_Response(200, "application/json", json_encode($data));//, self::jsonWriterSettings()));
    }

    private static function created($id)
    {
        return new REST_Response(201, "text/plain", (string) $id);
    }

    private static function deleted($id)
    {
        return new REST_Response(200, "text/plain", "Document with id $id deleted");
    }

    private static function verifyParameters(string $dbName, string $collectionName, string $id, bool $isIdRequired, ?string $data, bool $isDataRequired)
    {
        if (empty($dbName)) {
            return self::badRequest("Exception: Missing db_name parameter");
        }

        if (empty($collectionName)) {
            return self::badRequest("Exception: Missing collection_name parameter");
        }

        if ($isIdRequired) {
            if (empty($id)) {
                return self::badRequest("Exception: Missing id parameter");
            } /*elseif (!ObjectId::isValid($id)) {
                return self::badRequest("Exception: Invalid id");
            }*/
        }

        if ($isDataRequired && $data === null) {
            return self::badRequest("Exception: Missing body");
        }

        return null;
    }

    public function ping()
    {
        return $this->crud_client->ping();
    }

    public function getAsync(string $dbName, string $collectionName, string $id)
    {
        try {
            $exception = self::verifyParameters($dbName, $collectionName, $id, true, null, false);
            if ($exception !== null) {
                return $exception;
            }

            $objectId = $id;
            $document = $this->crud_client->readAsync($dbName, $collectionName, $objectId);
            if ($document !== null) {
                return self::ok($document);
            }

            return self::notFound($objectId);
        } catch (\Exception $e) {
            return self::errorMessage($e);
        }
    }

    public function postAsync(string $dbName, string $collectionName, string $data)
    {
        try {
            $exception = self::verifyParameters($dbName, $collectionName, "", false, $data, true);
            if ($exception !== null) {
                return $exception;
            }

            $documentId = $this->crud_client->createAsync($dbName, $collectionName, new BSONDocument(json_decode($data, true)));
            return $documentId !== null && !empty($documentId)
                ? self::created($documentId)
                : self::errorMessageFromString("Failed to create document");
        } catch (\Exception $e) {
            return self::errorMessage($e);
        }
    }

    public function putAsync(string $dbName, string $collectionName, string $id, string $data)
    {
        try {
            $exception = self::verifyParameters($dbName, $collectionName, $id, true, $data, true);
            if ($exception !== null) {
                return $exception;
            }

            $objectId = $id;
            $document = $this->crud_client->updateAsync($dbName, $collectionName, $objectId, new BSONDocument(json_decode($data, true)));
            if ($document !== null) {
                return self::ok($document);
            }

            return self::notFound($objectId);
        } catch (\Exception $e) {
            return self::errorMessage($e);
        }
    }

    public function deleteAsync(string $dbName, string $collectionName, string $id)
    {
        try {
            $exception = self::verifyParameters($dbName, $collectionName, $id, true, null, false);
            if ($exception !== null) {
                return $exception;
            }

            $objectId = $id;
            $result = $this->crud_client->deleteAsync($dbName, $collectionName, $objectId);
            if ($result) {
                return self::deleted($id);
            }

            return self::notFound($objectId);
        } catch (\Exception $e) {
            return self::errorMessage($e);
        }
    }
}
