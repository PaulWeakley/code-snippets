# Builder stage: use the official Rust image to compile the application
FROM rust:latest AS builder

# Set the working directory inside the container
WORKDIR /usr/src/app

# Copy Cargo files separately to leverage caching
COPY ./src/Cargo.toml ./src/Cargo.lock ./
RUN mkdir src && echo "fn main() {}" > src/main.rs
# Pre-build dependencies (this helps with caching)
RUN cargo build --release || true

# Copy the source code into the container
COPY ./src/src ./src

# Build the application in release mode
RUN cargo build --release

# Runtime stage: use a minimal base image
FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*
# Copy the compiled binary from the builder stage
COPY --from=builder /usr/src/app/target/release/rust-mongodb-rest-api /usr/local/bin/
RUN chmod +x /usr/local/bin/rust-mongodb-rest-api

# Make port 80 available to the world outside this container
EXPOSE 80

# Start and enable SSH
# Install SSH and setup environment
# Expose SSH port for Azure
# RUN apt-get update \
#     && apt-get install -y --no-install-recommends dialog openssh-server supervisor \
#     && mkdir -p /var/run/sshd \
#     && ssh-keygen -A \
#     && echo "root:Docker!" | chpasswd \
#     && chmod 600 /etc/ssh/ssh_host_* \
#     && chmod 644 /etc/ssh/sshd_config \
#     && apt-get clean && rm -rf /var/lib/apt/lists/*
# COPY sshd_config /etc/ssh/sshd_config
# COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf
# EXPOSE 2222

# Set the entrypoint to the compiled binary
CMD ["/usr/local/bin/rust-mongodb-rest-api"]
# Start Supervisor to run with SSH enabled
#CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]

