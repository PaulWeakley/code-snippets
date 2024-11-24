package com.api.mongodb.rest.controller;

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

    @Autowired
    public MongoDB_REST_Controller(IMongoDB_Client_Builder mongoDB_Client_Builder) {
        this.mongoDB_Client_Builder = mongoDB_Client_Builder;
    }

    private MongoDB_REST_Client createMongoDBClient() {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client(mongoDB_Client_Builder));
    }

    private ResponseEntity<String> toResponseEntity(REST_Response response) {
        return ResponseEntity.status(response.getStatusCode())
                .contentType(MediaType.parseMediaType(response.getContentType()))
                .body(response.getBody());
    }

    @GetMapping("/{db_name}/{collection_name}/{id}")
    public ResponseEntity<String> get(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id) throws Exception {
        return toResponseEntity(createMongoDBClient().getAsync(db_name, collection_name, id).get());
    }

    @PostMapping("/{db_name}/{collection_name}")
    public ResponseEntity<String> post(@PathVariable String db_name, @PathVariable String collection_name, @RequestBody Object body) throws Exception {
        ObjectMapper objectMapper = new ObjectMapper();
        return toResponseEntity(createMongoDBClient().postAsync(db_name, collection_name, objectMapper.writeValueAsString(body)).get());
    }

    @PutMapping("/{db_name}/{collection_name}/{id}")
    @PatchMapping("/{db_name}/{collection_name}/{id}")
    public ResponseEntity<String> put(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id, @RequestBody Object body) throws Exception {
        ObjectMapper objectMapper = new ObjectMapper();
        return toResponseEntity(createMongoDBClient().putAsync(db_name, collection_name, id, objectMapper.writeValueAsString(body)).get());
    }

    @DeleteMapping("/{db_name}/{collection_name}/{id}")
    public ResponseEntity<String> delete(@PathVariable String db_name, @PathVariable String collection_name, @PathVariable String id) throws Exception {
        return toResponseEntity(createMongoDBClient().deleteAsync(db_name, collection_name, id).get());
    }
}