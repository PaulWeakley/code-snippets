package com.api.mongodb.rest.service.mongodb.crud;

import com.mongodb.client.MongoClient;
import com.mongodb.client.MongoClients;
import com.mongodb.ConnectionString;
import com.mongodb.MongoClientSettings;

public class MongoDB_Client_Builder implements IMongoDB_Client_Builder {
    private final MongoClientSettings mongoClientSettings;

    public MongoDB_Client_Builder(String connectionString) {
        this.mongoClientSettings = MongoClientSettings.builder()
            .applyConnectionString(new ConnectionString(connectionString))
            .build();
    }

    public MongoDB_Client_Builder(MongoClientSettings mongoClientSettings) {
        this.mongoClientSettings = mongoClientSettings;
    }

    public MongoClient build() {
        System.out.println("MongoDB Client Settings: " + mongoClientSettings);
        return MongoClients.create(mongoClientSettings);
    }
}
