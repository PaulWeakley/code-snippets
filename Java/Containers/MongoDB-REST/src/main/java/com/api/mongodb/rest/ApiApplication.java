package com.api.mongodb.rest;

import com.google.gson.JsonObject;
import com.google.gson.JsonParser;

import java.io.FileReader;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Lazy;
import org.springframework.scheduling.annotation.EnableAsync;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.PathMatchConfigurer;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import org.springframework.web.util.pattern.PathPatternParser;

import com.api.mongodb.rest.service.mongodb.crud.IMongoDB_Client_Builder;
import com.api.mongodb.rest.service.mongodb.crud.MongoDB_Client_Builder;

@SpringBootApplication
public class ApiApplication {
    public static void main(String[] args) {
        SpringApplication.run(ApiApplication.class, args);
    }
}

@RestController
@RequestMapping("/api")
class ApiController {
    @GetMapping("/hello")
    public String sayHello() {
        return "Hello, Dockerized Java REST API!";
    }
}

@Configuration
@EnableAsync
class ApiConfiguration {
    @Bean
    @Lazy
    public IMongoDB_Client_Builder mongoDB_Client_Builder() {
        JsonObject root = null;
        try (FileReader reader = new FileReader("appsettings.json")) {
            root = JsonParser.parseReader(reader).getAsJsonObject();
        } catch (Throwable e) {
            throw new RuntimeException("Error reading appsettings.json", e);
        }
        JsonObject mongodb = root.getAsJsonObject("MongoDB");
        String mongoDbServer = System.getenv("MONGODB_SERVER");
        String mongoDbUsername = System.getenv("MONGODB_USERNAME");
        String mongoDbPassword = System.getenv("MONGODB_PASSWORD");
        boolean mongoDbRetryWrites = mongodb.get("retryWrites").getAsBoolean();
        String mongoDbWriteConcern = mongodb.get("w").getAsString();
        String mongoDbAppName = mongodb.get("appName").getAsString();

        String mongodb_connection_string = String.format(
            "mongodb+srv://%s:%s@%s/?retryWrites=%b&w=%s&appName=%s",
            mongoDbUsername, mongoDbPassword, mongoDbServer, mongoDbRetryWrites, mongoDbWriteConcern, mongoDbAppName
        );
        System.out.println("MongoDB Connection String: " + mongodb_connection_string);

        return new MongoDB_Client_Builder(mongodb_connection_string);
    }
}

@Configuration
class PathPatternParserConfig implements WebMvcConfigurer {
    @Override
    public void configurePathMatch(PathMatchConfigurer configurer) {
        PathPatternParser parser = new PathPatternParser();
        parser.setMatchOptionalTrailingSeparator(true); // Enable matching with or without trailing slash
        configurer.setPatternParser(parser);
    }
}
/*

@Configuration
class WebConfig implements WebMvcConfigurer {
    @Override
    public void configurePathMatch(PathMatchConfigurer configurer) {
        configurer.setPatternParser(new PathPatternParser().setMatchOptionalTrailingSlash(true)); // Enable matching with or without trailing slash
    }
}*/