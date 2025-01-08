<?php

namespace App\Services\MongoDB\REST;

class REST_Response
{
    private $statusCode;
    private $contentType;
    private $body;

    public function __construct($statusCode, $contentType, $body)
    {
        $this->statusCode = $statusCode;
        $this->contentType = $contentType;
        $this->body = $body;
    }

    public function getStatusCode()
    {
        return $this->statusCode;
    }

    public function getContentType()
    {
        return $this->contentType;
    }

    public function getBody()
    {
        return $this->body;
    }

    public function __toString()
    {
        return sprintf(
            "RestResponse(status_code=%d, content_type='%s', body='%s')",
            $this->statusCode,
            $this->contentType,
            $this->body
        );
    }

    public function toArray()
    {
        return [
            'status_code' => $this->statusCode,
            'content_type' => $this->contentType,
            'body' => $this->body
        ];
    }
}