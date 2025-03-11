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
# Expose the port the app runs on
EXPOSE 80

# Run the binary
CMD ["/usr/local/bin/rust-mongodb-rest-api"]
