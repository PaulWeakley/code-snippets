class REST_Response:
    def __init__(self, status_code: int, content_type: str, body: str):
        self.status_code = status_code
        self.content_type = content_type
        self.body = body

    def __repr__(self):
        return f"RestResponse(status_code={self.status_code}, content_type='{self.content_type}', body='{self.body}')"

    def to_dict(self):
        return {
            "status_code": self.status_code,
            "content_type": self.content_type,
            "body": self.body
        }