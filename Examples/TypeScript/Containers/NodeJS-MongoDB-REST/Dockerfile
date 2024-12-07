# Stage 1: Build the TypeScript code
FROM node:18 AS builder

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build

# Stage 2: Run the application
FROM node:18

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci --only=production

COPY --from=builder /app/dist ./dist

# Security: Create a non-root user
RUN useradd --group node --uid 1001 --create-home appuser \
    && chown -R appuser:node /app
USER appuser

EXPOSE 80

CMD ["node", "./dist/otel-instrumentation.js", "dist/src/server.js"]