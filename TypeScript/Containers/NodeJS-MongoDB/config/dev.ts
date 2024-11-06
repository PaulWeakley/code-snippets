const developmentConfig = {
    mongodb: {
      retryWrites: true,
      w: 'majority',
      appName: 'Cluster0',
    },
  };
  
  export default developmentConfig;