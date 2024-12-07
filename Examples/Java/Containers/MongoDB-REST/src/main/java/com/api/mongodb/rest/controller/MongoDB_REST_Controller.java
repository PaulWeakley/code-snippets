package com.api.mongodb.rest.controller;

import java.util.concurrent.CompletableFuture;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.api.mongodb.rest.service.mongodb.crud.*;
import com.api.mongodb.rest.service.mongodb.rest.*;
import com.fasterxml.jackson.databind.ObjectMapper;

@RestController
@RequestMapping("/api/mongodb")
public class MongoDB_REST_Controller {

    private final IMongoDB_Client_Builder mongoDB_Client_Builder;
    private final ObjectMapper objectMapper;

    @Autowired
    public MongoDB_REST_Controller(IMongoDB_Client_Builder mongoDB_Client_Builder, ObjectMapper objectMapper) {
        this.mongoDB_Client_Builder = mongoDB_Client_Builder;
        this.objectMapper = objectMapper;
    }

    private MongoDB_REST_Client createMongoDBClient() {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client(mongoDB_Client_Builder));
    }

    private CompletableFuture<ResponseEntity<String>> toResponseEntity(CompletableFuture<REST_Response> response) {
        return response.thenApply(restResponse -> 
            ResponseEntity.status(restResponse.getStatusCode())
            .contentType(MediaType.parseMediaType(restResponse.getContentType()))
            .body(restResponse.getBody())
        ).exceptionally(ex -> 
            ResponseEntity.status(500).body("An error occurred: " + ex.getMessage())
        );
    }

    @GetMapping("/{db_name}/{collection_name}/{id}")
    public CompletableFuture<ResponseEntity<String>> get(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id) {
        return toResponseEntity(createMongoDBClient().getAsync(db_name, collection_name, id));
    }

    @PostMapping("/{db_name}/{collection_name}")
    public CompletableFuture<ResponseEntity<String>> post(@PathVariable String db_name, @PathVariable String collection_name, @RequestBody Object body) throws Exception {
        return toResponseEntity(createMongoDBClient().postAsync(db_name, collection_name, this.objectMapper.writeValueAsString(body)));
    }

    @PutMapping("/{db_name}/{collection_name}/{id}")
    @PatchMapping("/{db_name}/{collection_name}/{id}")
    public CompletableFuture<ResponseEntity<String>> put(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id, @RequestBody Object body) throws Exception {
        return toResponseEntity(createMongoDBClient().putAsync(db_name, collection_name, id, this.objectMapper.writeValueAsString(body)));
    }

    @DeleteMapping("/{db_name}/{collection_name}/{id}")
    public CompletableFuture<ResponseEntity<String>> delete(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id) {
        return toResponseEntity(createMongoDBClient().deleteAsync(db_name, collection_name, id));
    }
}