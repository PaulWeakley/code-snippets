class MongoDBConfig:
    def __init__(self, username, password, server, retryWrites, w, appName):
        self.username = username
        self.password = password
        self.server = server
        self.retryWrites = retryWrites
        self.w = w
        self.appName = appName

    def to_uri(self) -> str:
        return f"mongodb+srv://{self.username}:{self.password}@{self.server}/?retryWrites={self.retryWrites}&w={self.w}&appName={self.appName}"