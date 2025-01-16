package com.api.mongodb.rest.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import com.api.mongodb.rest.service.mongodb.crud.MongoDB_CRUD_Client;
import com.api.mongodb.rest.telemetry.Telemetry;
import org.bson.BsonDocument;
import org.bson.BsonInt64;
import org.bson.types.ObjectId;

import java.time.Instant;
import java.time.Duration;
import java.time.LocalDateTime;
import java.time.ZoneOffset;

@RestController
@RequestMapping("/api/telemetry")
public class TelemetryController {
    @PostMapping("/{raw_start}")
    public ResponseEntity<Telemetry> postTelemetry(@PathVariable double raw_start, @RequestBody String requestBody) throws Exception {
        Instant start = Instant.ofEpochSecond((long) raw_start)
                .plusMillis((long) ((raw_start % 1) * 1000));
        double containerTime = Instant.now().toEpochMilli() / 1000.0;
        Duration sslHandshake = Duration.between(start, Instant.now());
        Instant start_ref = Instant.now();

        Instant measure = Instant.now();
        BsonDocument body = BsonDocument.parse(requestBody);
        Duration deserialization = Duration.between(measure, Instant.now());

        String db_name = "test";
        String collection_name = "source";
        measure = Instant.now();
        MongoDB_CRUD_Client client = new MongoDB_CRUD_Client();
        ObjectId document_id = client.createAsync(db_name, collection_name, body).get();
        Duration createObject = Duration.between(measure, Instant.now());

        measure = Instant.now();
        BsonDocument document = client.readAsync(db_name, collection_name, document_id).get();
        Duration getObject = Duration.between(measure, Instant.now());

        measure = Instant.now();
        String jsonData = document.toJson();
        Duration serialization = Duration.between(measure, Instant.now());

        measure = Instant.now();
        for (int i = 0; i < 15; i++) {
            client.updateAsync(db_name, collection_name, document_id, new BsonDocument("time", new BsonInt64(Instant.now().toEpochMilli()))).get();
        }
        Duration highIOps = Duration.between(measure, Instant.now());

        measure = Instant.now();
        client.deleteAsync(db_name, collection_name, document_id).get();
        Duration deleteObject = Duration.between(measure, Instant.now());

        Duration total = Duration.between(start_ref, Instant.now());

        Telemetry telemetry = new Telemetry();
        telemetry.setStart(raw_start);
        telemetry.setContainerTime(containerTime);
        telemetry.setSslHandshake(sslHandshake.toNanos() / 1_000_000.0);
        telemetry.setDeserialization(deserialization.toNanos() / 1_000_000.0);
        telemetry.setCreateObject(createObject.toNanos() / 1_000_000.0);
        telemetry.setGetObject(getObject.toNanos() / 1_000_000.0);
        telemetry.setSerialization(serialization.toNanos() / 1_000_000.0);
        telemetry.setHighIOps(highIOps.toNanos() / 1_000_000.0);
        telemetry.setDeleteObject(deleteObject.toNanos() / 1_000_000.0);
        telemetry.setTotal(total.toNanos() / 1_000_000.0);

        return ResponseEntity.ok(telemetry);
    }
}
