namespace Kafka.Client;

public class CloudEventHeaders : Dictionary<string, string>
{
    static public void ExtractDefaultHeaders(Dictionary<string,string>? headers,
        out string? specversion, out string? type, out string? source, out string? subject)
    {
        specversion = type = source = subject = null;
        headers?.TryGetValue("specversion", out specversion);
        headers?.TryGetValue("type", out type);
        headers?.TryGetValue("source", out source);
        headers?.TryGetValue("subject", out subject);
    }
    public CloudEventHeaders(string? specVersion = null, string? type = null, string? source = null, string? subject = null)
    {
        Add("specversion", specVersion ?? "");
        Add("type", type ?? "");
        Add("source", source ?? "");
        Add("subject", subject ?? "");
    }

    public string SpecVersion
    {
        get => this["specversion"];
        set => this["specversion"] = value;
    }

    public string Type
    {
        get => this["type"];
        set => this["type"] = value;
    }

    public string Source
    {
        get => this["source"];
        set => this["source"] = value;
    }

    public string Subject
    {
        get => this["subject"];
        set => this["subject"] = value;
    }
}