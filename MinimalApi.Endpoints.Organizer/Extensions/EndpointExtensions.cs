
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints.Organizer.Abstractions;
using System.Reflection;

namespace MinimalApi.Endpoints.Organizer.Extensions;
public static class EndpointExtensions
{
    public static void MapEndpoints(
        this WebApplication app,
        Assembly assembly,
        Action<EndpointRegistrationOptions>? configure = null
    )
    {
        EndpointRegistrationOptions? options = new();
        configure?.Invoke(options);
        List<Type>? endpointTypes = FindEndpoints(assembly);

        foreach (Type? endpointType in endpointTypes)
        {
            RegisterEndpoint(app, endpointType, options);
        }
    }

    private static List<Type> FindEndpoints(Assembly assembly)
    {
        return assembly.GetTypes()
                       .Where(x =>
                            x.IsClass &&
                            !x.IsAbstract &&
                            x.GetInterfaces()
                                .Any(i =>
                                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEndpoint<>
                                ))
                       ).ToList();
    }

    private static void RegisterEndpoint(
        WebApplication app,
        Type endpointType,
        EndpointRegistrationOptions? endpointRegistrationOptions
    )
    {
        Type? endpointInterface = endpointType.GetInterfaces()
                                                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEndpoint<>));

        Type? configurationType = endpointInterface.GetGenericArguments()[0];

        var configInstance = (EndpointsConfiguration)ActivatorUtilities.CreateInstance(app.Services, configurationType);

        RouteGroupBuilder routeGroup = ConfigureRouteGroup(app, configInstance, endpointRegistrationOptions);

        object? endpointInstance = ActivatorUtilities.CreateInstance(app.Services, endpointType);

        MethodInfo? mapMethod = endpointInterface.GetMethod("MapEndpoint");

        mapMethod?.Invoke(endpointInstance, [routeGroup]);
    }

    private static RouteGroupBuilder ConfigureRouteGroup(
        WebApplication app,
        EndpointsConfiguration configInstance,
        EndpointRegistrationOptions? endpointRegistrationOptions
    )
    {
        Type configurationType = configInstance.GetType();

        string prefix = ConfigurePrefix(configInstance, endpointRegistrationOptions);

        RouteGroupBuilder routeGroup = app.MapGroup(prefix);

        ConfigureTags(configInstance, routeGroup);
        ConfigureOpenApi(configInstance.IncludeOpenApi, routeGroup);

        endpointRegistrationOptions?.ConfigureGroup?.Invoke(routeGroup);

        return routeGroup;
    }

    private static string ConfigurePrefix(
        EndpointsConfiguration configInstance,
        EndpointRegistrationOptions? endpointRegistrationOptions
    )
    {
        List<string> prefixSegments = [];

        if (!string.IsNullOrEmpty(endpointRegistrationOptions?.BasePrefix))
        {
            prefixSegments.Add(endpointRegistrationOptions.BasePrefix);
        }
        if (!string.IsNullOrEmpty(configInstance.Prefix))
        {
            prefixSegments.Add(configInstance.Prefix);
        }
        if (prefixSegments.Count == 0)
        {
            prefixSegments.Add("/");
        }

        string prefix = string.Join("/", prefixSegments.Select(s => s.Trim('/')));
        return prefix;
    }

    private static void ConfigureTags(
        EndpointsConfiguration configInstance,
        RouteGroupBuilder routeGroup
    )
    {
        string[]? tags = configInstance.Tags;
        if (tags != null && tags.Length > 0)
        {
            routeGroup.WithTags(tags);
        }
    }

    private static void ConfigureOpenApi(bool includeInOpenApi, RouteGroupBuilder routeGroup)
    {
        if (includeInOpenApi)
        {
            routeGroup.WithOpenApi();
        }
    }
}