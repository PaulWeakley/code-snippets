<?php

require_once '../CRUD/MongoDB_CRUD_Client.php';

class MongoDB_REST_Client {
    private $crud_client;

    public function __construct($mongodb_crud_client) {
        $this->crud_client = $mongodb_crud_client;
    }

    private function bad_request($message) {
        return [
            'statusCode' => 400,
            'headers' => ['Content-Type' => 'text/plain'],
            'body' => json_encode($message)
        ];
    }

    private function error_message($e) {
        return [
            'statusCode' => 500,
            'headers' => ['Content-Type' => 'text/plain'],
            'body' => $e->getMessage()
        ];
    }

    private function not_found($id) {
        return [
            'statusCode' => 404,
            'headers' => ['Content-Type' => 'text/plain'],
            'body' => "Error: Document with id $id not found"
        ];
    }

    private function ok($data) {
        return [
            'statusCode' => 200,
            'headers' => ['Content-Type' => 'application/json'],
            'body' => json_encode($data)
        ];
    }

    private function verify_parameters($db_name, $collection_name, $id, $is_id_required, $data, $is_data_required) {
        if ($db_name === null) {
            return $this->bad_request('Error: Missing db_name parameter');
        }
        if ($collection_name === null) {
            return $this->bad_request('Error: Missing collection_name parameter');
        }
        if ($is_id_required) {
            if ($id === null) {
                return $this->bad_request('Error: Missing id parameter');
            } elseif (!MongoDB\BSON\ObjectId::isValid($id)) {
                return $this->bad_request('Error: Invalid id');
            }
        }
        if ($is_data_required && $data === null) {
            return $this->bad_request('Error: Missing body');
        }
        return null;
    }

    public function ping() {
        $this->crud_client->ping();
    }

    public function get($db_name, $collection_name, $id) {
        try {
            $error = $this->verify_parameters($db_name, $collection_name, $id, true, null, false);
            if ($error !== null) {
                return $error;
            }

            $document = $this->crud_client->read($db_name, $collection_name, new MongoDB\BSON\ObjectId($id));
            if ($document !== null) {
                return $this->ok($document);
            }
            return $this->not_found($id);
        } catch (Exception $e) {
            return $this->error_message($e);
        }
    }

    public function post($db_name, $collection_name, $data) {
        try {
            $error = $this->verify_parameters($db_name, $collection_name, null, false, $data, true);
            if ($error !== null) {
                return $error;
            }

            $document_id = $this->crud_client->create($db_name, $collection_name, $data);
            if ($document_id !== null) {
                return [
                    'statusCode' => 201,
                    'body' => (string)$document_id
                ];
            }
            return [
                'statusCode' => 500,
                'body' => 'Failed to create document'
            ];
        } catch (Exception $e) {
            return $this->error_message($e);
        }
    }

    public function put($db_name, $collection_name, $id, $data) {
        try {
            $error = $this->verify_parameters($db_name, $collection_name, $id, true, $data, true);
            if ($error !== null) {
                return $error;
            }

            $document = $this->crud_client->update($db_name, $collection_name, new MongoDB\BSON\ObjectId($id), $data);
            if ($document !== null) {
                return $this->ok($document);
            }
            return $this->not_found($id);
        } catch (Exception $e) {
            return $this->error_message($e);
        }
    }

    public function delete($db_name, $collection_name, $id) {
        try {
            $error = $this->verify_parameters($db_name, $collection_name, $id, true, null, false);
            if ($error !== null) {
                return $error;
            }

            $result = $this->crud_client->delete($db_name, $collection_name, new MongoDB\BSON\ObjectId($id));
            if ($result) {
                return [
                    'statusCode' => 200,
                    'body' => "Document with id $id deleted"
                ];
            }
            return $this->not_found($id);
        } catch (Exception $e) {
            return $this->error_message($e);
        }
    }
}