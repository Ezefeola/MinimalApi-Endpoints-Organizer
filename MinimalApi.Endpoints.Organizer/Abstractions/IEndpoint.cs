using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApi.Endpoints.Organizer.Abstractions;
public interface IEndpoint<TConfiguration> where TConfiguration : EndpointsConfiguration
{
    RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app);
}