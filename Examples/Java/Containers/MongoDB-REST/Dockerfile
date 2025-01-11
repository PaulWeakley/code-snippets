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

# Start and enable SSH
# Install SSH and setup environment
# RUN apt-get update \
#     && apt-get install -y --no-install-recommends dialog openssh-server supervisor \
#     && mkdir -p /var/run/sshd \
#     && ssh-keygen -A \
#     && echo "root:Docker!" | chpasswd \
#     && chmod 600 /etc/ssh/ssh_host_* \
#     && chmod 644 /etc/ssh/sshd_config \
#     && apt-get clean && rm -rf /var/lib/apt/lists/*
# Copy SSH configuration file
# COPY sshd_config /etc/ssh/sshd_config
# COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
# Expose SSH port for Azure
# EXPOSE 2222

# Expose the application's port
EXPOSE 8080


# Command to run the application with OpenTelemetry Java agent
CMD ["java", "-jar", "app.jar"]
# Start Supervisor to run with SSH enabled
# CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]