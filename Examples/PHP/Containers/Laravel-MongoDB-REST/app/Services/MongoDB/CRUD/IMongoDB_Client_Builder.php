<?php

namespace App\Services\MongoDB\CRUD;

interface IMongoDB_Client_Builder
{
    public function build(): IMongoDB_CRUD_Client;
}