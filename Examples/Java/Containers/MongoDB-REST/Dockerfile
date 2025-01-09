# Stage 1: Build the application using Maven
FROM maven:3.9.9-eclipse-temurin-17 AS build

# Set the working directory
WORKDIR /app

# Copy the Maven project files
COPY pom.xml .
COPY src ./src

# Run Maven to package the application
RUN mvn clean package -DskipTests

# Stage 2: Create a minimal runtime image
FROM openjdk:17-jdk-slim

# Set the working directory
WORKDIR /app

# Copy the built JAR file from the build stage
COPY application.properties .
COPY appsettings.json .
COPY --from=build /app/target/*.jar app.jar

# Install wget and download the Datadog Java agent
# RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/* \
# 	&& wget -O dd-java-agent.jar 'https://dtdg.co/latest-java-tracer'
# Clean up wget
# RUN apt-get remove -y wget && apt-get autoremove -y && apt-get clean

# Expose the application's port
EXPOSE 8080


# Command to run the application with OpenTelemetry Java agent
CMD ["java", "-jar", "app.jar"]