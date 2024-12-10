# Stage 1: Build the TypeScript code
FROM node:22 AS builder

WORKDIR /app

# Install dependencies
COPY package.json package-lock.json ./
RUN npm ci

# Copy source code and build
COPY . ./
RUN npm run build

# Stage 2: Run the application
FROM node:22

WORKDIR /app

# Install only production dependencies
COPY package.json package-lock.json ./
RUN npm ci --only=production

# Copy built files from the builder stage
COPY --from=builder /app/dist ./dist

# Security: Create and use a non-root user
RUN useradd --group node --uid 1001 --create-home appuser \
    && chown -R appuser:node ./dist
USER appuser

# Expose the application port
EXPOSE 80

# Run the application
CMD ["node", "./dist/src/server.js"]