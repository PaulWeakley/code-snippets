# Stage 1: Build the TypeScript code
FROM node:18 AS builder

# Set working directory
WORKDIR /app

# Copy package.json and install dependencies
COPY package.json package-lock.json ./
RUN npm install

# Copy the source code
COPY . .

# Copy .env file
COPY .env .env

# Build the TypeScript code
RUN npm run build

# Stage 2: Run the application
FROM node:18

# Set working directory
WORKDIR /app

# Copy only the compiled files and dependencies
COPY --from=builder /app/dist ./dist
COPY package.json package-lock.json ./
RUN npm install --only=production

# Expose the port the app runs on
EXPOSE 3000

# Run the app
CMD ["node", "dist/src/app.js"]
