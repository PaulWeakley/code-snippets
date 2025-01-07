class MongoDB_Config:
    def __init__(self, username, password, server, retryWrites, writeConcern, appName, minPoolSize, maxPoolSize, waitQueueTimeoutMS):
        self.username = username
        self.password = password
        self.server = server
        self.retryWrites = retryWrites
        self.writeConcern = writeConcern
        self.appName = appName
        self.minPoolSize = minPoolSize
        self.maxPoolSize = maxPoolSize
        self.waitQueueTimeoutMS = waitQueueTimeoutMS

    def to_uri(self) -> str:
        return f"mongodb+srv://{self.username}:{self.password}@{self.server}/?retryWrites={self.retryWrites}&w={self.writeConcern}&appName={self.appName}&minPoolSize={self.minPoolSize}&maxPoolSize={self.maxPoolSize}&waitQueueTimeoutMS={self.waitQueueTimeoutMS}"