package com.api.mongodb.rest;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Configuration;

import org.springframework.web.servlet.config.annotation.PathMatchConfigurer;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import org.springframework.web.util.pattern.PathPatternParser;

import com.api.mongodb.rest.service.mongodb.crud.MongoDB_CRUD_Client;

@SpringBootApplication
public class ApiApplication {
    public static void main(String[] args) {
        String mongodb_server = System.getenv().getOrDefault("MONGODB_SERVER", "localhost");
        String mongodb_username = System.getenv().getOrDefault("MONGODB_USERNAME", "root");
        String mongodb_password = System.getenv().getOrDefault("MONGODB_PASSWORD", "password");
        String mongodb_retry_writes = System.getenv().getOrDefault("MONGODB_RETRY_WRITES", "true");
        String mongodb_write_concern = System.getenv().getOrDefault("MONGODB_WRITE_CONCERN", "majority");
        String mongodb_app_name = System.getenv().getOrDefault("MONGODB_APP_NAME", "MongoDB.REST");
        String mongodb_min_pool_size = System.getenv().getOrDefault("MONGODB_MIN_POOL_SIZE", "10");
        String mongodb_max_pool_size = System.getenv().getOrDefault("MONGODB_MAX_POOL_SIZE", "50");
        String mongodb_wait_queue_size = System.getenv().getOrDefault("MONGODB_WAIT_QUEUE_SIZE", "1000");

        String mongodb_connection_string = String.format(
            "mongodb+srv://%s:%s@%s/?retryWrites=%s&w=%s&appName=%s&minPoolSize=%s&maxPoolSize=%s&waitQueueSize=%s",
            mongodb_username, mongodb_password, mongodb_server, mongodb_retry_writes, mongodb_write_concern, mongodb_app_name,
            mongodb_min_pool_size, mongodb_max_pool_size, mongodb_wait_queue_size
        );

        MongoDB_CRUD_Client.initialize(mongodb_connection_string);
        SpringApplication.run(ApiApplication.class, args);
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