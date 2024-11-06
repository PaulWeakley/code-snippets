import IMongoDB_Config from './IMongoDB_Config';

class MongoDB_Config implements IMongoDB_Config {
    username: string;
    password: string;
    server: string;
    retryWrites: boolean;
    w: string;
    appName: string;

    constructor(username: string, password: string, server: string, retryWrites: boolean, w: string, appName: string) {
        this.username = username;
        this.password = password;
        this.server = server;
        this.retryWrites = retryWrites;
        this.w = w;
        this.appName = appName;
    }
    
    toUri(): string {
        return `mongodb+srv://${this.username}:${this.password}@${this.server}/?retryWrites=${this.retryWrites}&w=${this.w}&appName=${this.appName}`;
    }
}

export default MongoDB_Config;