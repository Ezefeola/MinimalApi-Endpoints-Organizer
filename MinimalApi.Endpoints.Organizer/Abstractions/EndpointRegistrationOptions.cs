using Microsoft.AspNetCore.Routing;

namespace MinimalApi.Endpoints.Organizer.Abstractions;
public class EndpointRegistrationOptions
{
    public string? BasePrefix { get; set; }
    public Action<RouteGroupBuilder>? ConfigureGroup { get; set; }
}