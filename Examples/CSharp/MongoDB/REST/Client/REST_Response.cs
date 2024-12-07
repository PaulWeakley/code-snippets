namespace MongoDB.REST.Client;

public class REST_Response
{
    public int StatusCode { get; }
    public string ContentType { get; }
    public string Body { get; }

    public REST_Response(int statusCode, string contentType, string body)
    {
        StatusCode = statusCode;
        ContentType = contentType;
        Body = body;
    }

    public override string ToString()
    {
        return $"RestResponse(status_code={StatusCode}, content_type='{ContentType}', body='{Body}')";
    }

    public Dictionary<string, object> ToDict()
    {
        return new Dictionary<string, object>
        {
            { "status_code", StatusCode },
            { "content_type", ContentType },
            { "body", Body }
        };
    }
}