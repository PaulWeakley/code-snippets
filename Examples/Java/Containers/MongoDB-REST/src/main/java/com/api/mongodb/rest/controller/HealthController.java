package com.api.mongodb.rest.controller;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import com.api.mongodb.rest.health.HealthResultEntry;
import com.api.mongodb.rest.health.HealthResults;
import com.api.mongodb.rest.service.mongodb.crud.*;
import com.api.mongodb.rest.service.mongodb.rest.*;

@RestController
@RequestMapping("/api/health")
public class HealthController {
    private MongoDB_REST_Client createMongoDBClient() {
        return new MongoDB_REST_Client(new MongoDB_CRUD_Client());
    }

    private CompletableFuture<HealthResultEntry> buildHealthCheckMongoDB(MongoDB_REST_Client mongodb_rest_client) {
        long start = System.currentTimeMillis();
        return mongodb_rest_client.ping()
            .thenApply(v -> new HealthResultEntry("mongodb", true, System.currentTimeMillis() - start, null))
            .exceptionally(e-> new HealthResultEntry("mongob", false, System.currentTimeMillis() - start, e.getMessage()));
    }

    private CompletableFuture<HealthResults> buildHealthCheckResponse(MongoDB_REST_Client mongodb_rest_client) {
        long start = System.currentTimeMillis();
        List<CompletableFuture<HealthResultEntry>> futures = new ArrayList<>();
        futures.add(buildHealthCheckMongoDB(mongodb_rest_client));
        CompletableFuture<Void> allOf = CompletableFuture.allOf(futures.toArray(new CompletableFuture[0]));
        return allOf
            .thenApply(v ->  {
                List<HealthResultEntry> results = futures.stream()
                    .map(CompletableFuture::join)
                    .collect(Collectors.toList());
                return new HealthResults(System.currentTimeMillis() - start, results);
            });
    }

    @GetMapping()
    @PostMapping()
    public ResponseEntity<HealthResults> getHealth() throws InterruptedException, ExecutionException {
        MongoDB_REST_Client client = createMongoDBClient();
        HealthResults healthResults = buildHealthCheckResponse(client).get();
        return ResponseEntity.status(healthResults.isHealthy() ? 200 : 500).body(healthResults);
    }
}