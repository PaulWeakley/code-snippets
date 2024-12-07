package com.api.mongodb.rest.service.mongodb.crud;

import com.mongodb.client.MongoClient;

public interface IMongoDB_Client_Builder {
    MongoClient build();
}