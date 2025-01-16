package com.api.mongodb.rest.telemetry;

import com.fasterxml.jackson.annotation.JsonProperty;

public class Telemetry {

    @JsonProperty("start")
    private double start;

    @JsonProperty("container_time")
    private double containerTime;

    @JsonProperty("ssl_handshake")
    private double sslHandshake;

    @JsonProperty("deserialization")
    private double deserialization;

    @JsonProperty("create_object")
    private double createObject;

    @JsonProperty("get_object")
    private double getObject;

    @JsonProperty("serialization")
    private double serialization;

    @JsonProperty("total")
    private double total;

    @JsonProperty("type")
    private final String type = "Java";

    @JsonProperty("high_iops")
    private double highIOps;

    @JsonProperty("delete_object")
    private double deleteObject;

    // Getters and Setters
    public double getStart() {
        return start;
    }

    public void setStart(double start) {
        this.start = start;
    }

    public double getContainerTime() {
        return containerTime;
    }

    public void setContainerTime(double containerTime) {
        this.containerTime = containerTime;
    }

    public double getSslHandshake() {
        return sslHandshake;
    }

    public void setSslHandshake(double sslHandshake) {
        this.sslHandshake = sslHandshake;
    }

    public double getDeserialization() {
        return deserialization;
    }

    public void setDeserialization(double deserialization) {
        this.deserialization = deserialization;
    }

    public double getCreateObject() {
        return createObject;
    }

    public void setCreateObject(double createObject) {
        this.createObject = createObject;
    }

    public double getGetObject() {
        return getObject;
    }

    public void setGetObject(double getObject) {
        this.getObject = getObject;
    }

    public double getSerialization() {
        return serialization;
    }

    public void setSerialization(double serialization) {
        this.serialization = serialization;
    }

    public double getTotal() {
        return total;
    }

    public void setTotal(double total) {
        this.total = total;
    }

    public String getType() {
        return type;
    }

    public double getHighIOps() {
        return highIOps;
    }

    public void setHighIOps(double highIOps) {
        this.highIOps = highIOps;
    }

    public double getDeleteObject() {
        return deleteObject;
    }

    public void setDeleteObject(double deleteObject) {
        this.deleteObject = deleteObject;
    }
}