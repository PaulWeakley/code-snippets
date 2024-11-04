class REST_Response {
    status_code: number;
    content_type: string;
    body: string;

    constructor(status_code: number, content_type: string, body: string) {
        this.status_code = status_code;
        this.content_type = content_type;
        this.body = body;
    }

    toString(): string {
        return `RestResponse(status_code=${this.status_code}, content_type='${this.content_type}', body='${this.body}')`;
    }

    to_dict(): { status_code: number; content_type: string; body: string } {
        return {
            status_code: this.status_code,
            content_type: this.content_type,
            body: this.body
        };
    }
}

export default REST_Response;