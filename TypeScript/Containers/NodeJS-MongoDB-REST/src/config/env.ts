import dotenv from 'dotenv';

// Load environment variables from .env file
dotenv.config();

export const env = {
    MONGODB_USERNAME: process.env.MONGODB_USERNAME || '',
    MONGODB_PASSWORD: process.env.MONGODB_PASSWORD || '',
    MONGODB_SERVER: process.env.MONGODB_SERVER || ''
};