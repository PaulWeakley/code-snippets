<?php

namespace App\Services\MongoDB\CRUD;

class MongoDB_Config {
    private $username;
    private $password;
    private $server;
    private $retryWrites;
    private $w;
    private $appName;

    public function __construct($username, $password, $server, $retryWrites, $w, $appName) {
        $this->username = $username;
        $this->password = $password;
        $this->server = $server;
        $this->retryWrites = $retryWrites;
        $this->w = $w;
        $this->appName = $appName;
    }

    public function toUri() {
        return "mongodb+srv://{$this->username}:{$this->password}@{$this->server}/?retryWrites={$this->retryWrites}&w={$this->w}&appName={$this->appName}";
    }
}