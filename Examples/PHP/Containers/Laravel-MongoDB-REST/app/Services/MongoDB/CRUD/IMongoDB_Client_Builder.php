<?php

namespace App\Services\MongoDB\CRUD;

interface IMongoDB_Client_Builder
{
    public function build(): \MongoDB\Client;
}