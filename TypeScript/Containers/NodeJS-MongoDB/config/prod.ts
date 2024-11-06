const productionConfig = {
    mongodb: {
        retryWrites: true,
        w: 'majority',
        appName: 'Cluster0',
      },
  };
  
  export default productionConfig;