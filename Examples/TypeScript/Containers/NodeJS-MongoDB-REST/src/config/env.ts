import dotenv from 'dotenv';

// Load environment variables from .env file
dotenv.config();

export const env = {
    MONGODB_USERNAME: process.env.MONGODB_USERNAME || '',
    MONGODB_PASSWORD: process.env.MONGODB_PASSWORD || '',
    MONGODB_SERVER: process.env.MONGODB_SERVER || '',
    MONGODB_APP_NAME: process.env.MONGODB_APP_NAME || '',
    MONGODB_RETRY_WRITES: process.env.MONGODB_RETRY_WRITES || 'true',
    MONGODB_WRITE_CONCERN: process.env.MONGODB_WRITE_CONCERN || 'majority',
    MONGODB_MIN_POOL_SIZE: process.env.MONGODB_MIN_POOL_SIZE || '10',
    MONGODB_MAX_POOL_SIZE: process.env.MONGODB_MAX_POOL_SIZE || '50',
    MONGODB_WAIT_QUEUE_TIMEOUT_MS: process.env.MONGODB_WAIT_QUEUE_TIMEOUT_MS || '1000',
};