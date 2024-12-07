package com.api.mongodb.rest.service.mongodb.rest;

import java.util.HashMap;
import java.util.Map;

public class REST_Response {
    private final int statusCode;
    private final String contentType;
    private final String body;

    public REST_Response(int statusCode, String contentType, String body) {
        this.statusCode = statusCode;
        this.contentType = contentType;
        this.body = body;
    }

    public int getStatusCode() {
        return statusCode;
    }

    public String getContentType() {
        return contentType;
    }

    public String getBody() {
        return body;
    }

    @Override
    public String toString() {
        return "REST_Response(status_code=" + statusCode + ", content_type='" + contentType + "', body='" + body + "')";
    }

    public Map<String, Object> toDict() {
        Map<String, Object> map = new HashMap<>();
        map.put("status_code", statusCode);
        map.put("content_type", contentType);
        map.put("body", body);
        return map;
    }
}