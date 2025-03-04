pub struct RESTResponse {
    pub status_code: u16,
    pub content_type: String,
    pub body: String,
}

impl RESTResponse {
    pub fn new(status_code: u16, content_type: &str, body: String) -> Self {
        RESTResponse {
            status_code,
            content_type: content_type.to_string(),
            body,
        }
    }
}