<?php

namespace App\Services\MongoDB\CRUD;

use MongoDB\Client;

class MongoDB_Client_Builder implements IMongoDB_Client_Builder
{
    private $connectionString;

    public function __construct($connectionString)
    {
        $this->connectionString = $connectionString;
    }

    public function build(): IMongoDB_CRUD_Client
    {
        return new MongoDB_CRUD_Client(new \MongoDB\Client($this->connectionString));
    }
}