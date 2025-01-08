class MongoDB_Config {
    username: string;
    password: string;
    server: string;
    retryWrites: boolean;
    writeConcern: string;
    appName: string;
    minPoolSize: number;
    maxPoolSize: number;
    waitQueueTimeoutMS: number;

    constructor(username: string, password: string, server: string, retryWrites: boolean, writeConcern: string, appName: string, minPoolSize: number, maxPoolSize: number, waitQueueTimeoutMS: number) {
        this.username = username;
        this.password = password;
        this.server = server;
        this.retryWrites = retryWrites;
        this.writeConcern = writeConcern;
        this.appName = appName;
        this.minPoolSize = minPoolSize;
        this.maxPoolSize = maxPoolSize;
        this.waitQueueTimeoutMS = waitQueueTimeoutMS;
    }
    
    toUri(): string {
        return `mongodb+srv://${this.username}:${this.password}@${this.server}/?retryWrites=${this.retryWrites}&w=${this.writeConcern}&appName=${this.appName}&minPoolSize=${this.minPoolSize}&maxPoolSize=${this.maxPoolSize}&waitQueueTimeoutMS=${this.waitQueueTimeoutMS}`;
    }
}

export default MongoDB_Config;