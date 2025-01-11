# Use the official .NET SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install Native AOT prerequisites
RUN apt-get update && apt-get install -y clang zlib1g-dev

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MongoDB.REST/MongoDB.REST.csproj", "./"]
RUN dotnet restore "MongoDB.REST.csproj"

# Copy the entire source code and build the application
COPY ./MongoDB.REST .
RUN dotnet publish "MongoDB.REST.csproj" -c Release -r linux-x64 --self-contained true /p:PublishAot=true -o /app/publish

# Final stage: Use a newer Debian-based minimal runtime image
FROM debian:bookworm-slim AS final

# Install runtime dependencies
RUN apt-get update && apt-get install -y libgcc1 libc6 libstdc++6 libssl3 ca-certificates && apt-get clean && rm -rf /var/lib/apt/lists/*
RUN update-ca-certificates

# Set the working directory
WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

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

# Make port 80 available to the world outside this container
EXPOSE 80

# Set the entrypoint to the compiled binary
ENTRYPOINT ["./MongoDB.REST"]
# Start Supervisor to run with SSH enabled
# CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]