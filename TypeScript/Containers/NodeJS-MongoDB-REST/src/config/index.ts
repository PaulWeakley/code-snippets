import developmentConfig from './dev';
import productionConfig from './prod';

const env = process.env.NODE_ENV || 'development';

const config = env === 'production' || env === 'prod' ? productionConfig : developmentConfig;

export default config;