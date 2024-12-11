<?php

namespace App\Services\MongoDB\CRUD;

class MongoDB_Client_Builder implements IMongoDB_Client_Builder
{
    private $connectionString;

    public function __construct(MongoDB_Config $config)
    {
        $this->connectionString = $config->toUri();
    }

    public function build(): \MongoDB\Client
    {
        return new \MongoDB\Client($this->connectionString);
    }
}