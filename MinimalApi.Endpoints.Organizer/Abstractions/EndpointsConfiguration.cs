namespace MinimalApi.Endpoints.Organizer.Abstractions;
public abstract class EndpointsConfiguration
{
    public string? Prefix { get; protected set; }
    public string[] Tags { get; protected set; } = [];
    public bool IncludeOpenApi { get; protected set; } = true;

    public EndpointsConfiguration WithPrefix(string prefix)
    {
        Prefix = prefix;
        return this;
    }
    public EndpointsConfiguration WithTags(params string[] tags)
    {
        Tags = tags;
        return this;
    }
    public EndpointsConfiguration WithOpenApi(bool hasOpenApi)
    {
        IncludeOpenApi = hasOpenApi;
        return this;
    }
}