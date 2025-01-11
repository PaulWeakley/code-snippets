# Stage 1: Build the TypeScript code
FROM node:22 AS builder

WORKDIR /app

# Install dependencies
COPY package.json package-lock.json ./
RUN npm ci

# Copy source code and build
COPY ./index.ts ./tsconfig.json ./
COPY ./src ./src
RUN npm run build

# Stage 2: Run the application
FROM node:22

WORKDIR /app

# Install only production dependencies
COPY package.json package-lock.json ./
RUN npm ci --only=production

# Copy built files from the builder stage
COPY --from=builder /app/dist ./dist

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

# Security: Create and use a non-root user (Comment out when running with SSH)
RUN useradd --group node --uid 1001 --create-home appuser \
    && chown -R appuser:node ./dist
USER appuser

# Expose the application port
EXPOSE 80

# Run the application
CMD ["node", "./dist/src/server.js"]
# Start Supervisor to run with SSH enabled
# CMD ["supervisord", "-c", "/etc/supervisor/supervisord.conf"]