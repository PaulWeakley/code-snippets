<?php

namespace App\Services\MongoDB\CRUD;

class MongoDB_Config {

    private static $instance = null;

    public static function getInstance() {
        if (self::$instance == null) {
            self::$instance = new self(
                config('mongodb.username'),
                config('mongodb.password'),
                config('mongodb.server'),
                config('mongodb.retryWrites'),
                config('mongodb.writeConcern'),
                config('mongodb.appName'),
                config('mongodb.minPoolSize'),
                config('mongodb.maxPoolSize'),
                config('mongodb.waitQueueTimeoutMS')
            );
        }
        return self::$instance;
    }


    private $username;
    private $password;
    private $server;
    private $retryWrites;
    private $writeConcern;
    private $appName;
    private $minPoolSize;
    private $maxPoolSize;
    private $waitQueueTimeoutMS;

    public function __construct($username, $password, $server, $retryWrites, $writeConcern, $appName, $minPoolSize, $maxPoolSize, $waitQueueTimeoutMS) {
        $this->username = $username;
        $this->password = $password;
        $this->server = $server;
        $this->retryWrites = $retryWrites;
        $this->writeConcern = $writeConcern;
        $this->appName = $appName;
        $this->minPoolSize = $minPoolSize;
        $this->maxPoolSize = $maxPoolSize;
        $this->waitQueueTimeoutMS = $waitQueueTimeoutMS;
    }

    public function toUri() {
        return "mongodb+srv://{$this->username}:{$this->password}@{$this->server}/?retryWrites={$this->retryWrites}&w={$this->writeConcern}&appName={$this->appName}&minPoolSize={$this->minPoolSize}&maxPoolSize={$this->maxPoolSize}&waitQueueTimeoutMS={$this->waitQueueTimeoutMS}";
    }
}