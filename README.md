# MinimalApi-Endpoints-Organizer
This package tries to solve Minimal API endpoints organization to make it easier and sustainable over time, allowing the user to create a base configuration class and pass it over every endpoint that implements the IEndpoint interface.

# MinimalApi-Endpoints-Organizer
This package tries to solve Minimal API endpoints organization to make it easier and sustainable over time, 
allowing the user to create a base configuration class and pass it over every endpoint that implements the IEndpoint interface.

# MinimalApi.Endpoints.Organizer

[![NuGet Version](https://img.shields.io/nuget/v/EFeola.MinimalApi.Endpoints.Organizer)](https://www.nuget.org/packages/EFeola.MinimalApi.Endpoints.Organizer/)

An elegant and robust abstraction for organizing and configuring Minimal API endpoints.

This package simplifies the creation and registration of endpoints by providing a convention-based approach, allowing you to define endpoint configuration in a single, reusable class.

### Key Features
* **Convention-Based Routing:** Automatically discovers and registers endpoints that implement the `IEndpoint` interface.
* **Fluent Configuration API:** Use a fluent builder pattern to easily set prefixes, tags, and OpenAPI settings for a group of endpoints.
* **Centralized Configuration:** Define all endpoint metadata (like `Tags` and `Prefix`) in a separate and clear configuration class.
* **OpenAPI Integration:** Easily include or exclude entire endpoint groups from your OpenAPI documentation.

### Installation

Install the package into your ASP.NET Core project using the .NET CLI:

## Getting Started
### To use the package follow these three simple steps:

#### Step 1: Define Your Endpoint Configuration
Create a configuration class that inherits from `EndpointsConfiguration` and use the fluent methods to set your desired properties.

```csharp
// In your Endpoints/UserEndpointsConfiguration.cs
using MinimalApi.Endpoints.Organizer.Abstractions;

public class UserEndpointsConfiguration : EndpointsConfiguration
{
    public UserEndpointsConfiguration()
    {
        .WithPrefix("/users/v2")
        .WithTags("Users", "User Management")
        .WithOpenApi(true);
    }
}
```
### Step 2: Implement the Endpoint
Create an endpoint class that implements IEndpoint<TConfiguration> where TConfiguration is your new configuration class.

```csharp
// In your Endpoints/GetUsersEndpoint.cs
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoints.Organizer.Abstractions;

public class GetUsersEndpoint : IEndpoint<UserEndpointsConfiguration>
{
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/", GetUsersHandler);
    }

    private static async Task<string> GetUsersHandler() 
    {
        return "¡users!";
    }
}
```

### Step 3: Register All Endpoints in Program.cs
Call the MapEndpoints extension method in your Program.cs file. It will automatically scan the specified assembly and register all endpoints.

```csharp

// In your Program.cs
using System.Reflection;
using MinimalApi.Endpoints.Organizer.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// This single line registers all your endpoints
app.MapEndpoints(Assembly.GetExecutingAssembly());

app.Run();
```

#### You can also pass a lambda for some extra configurations in the app.MapEndpoints after giving the desired Assembly, so this lets you configure a BasePrefix and Configure the endpoints group to add filters or global configurations:
```csharp
app.MapEndpoints(Assembly.GetExecutingAssembly(), options => 
{
    options.BasePrefix = "/api";
    options.ConfigureGroup = group => 
    {
        group.AddEndpointFilter<ResultHttpCodeFilter>();
        group.WithOpenApi();
    };
});

```

That's it! Your GetUsersEndpoint is now automatically available at /users/v2 with the Users and User Management tags.

For any further suggest or feature that you would like to see don't hesitate to contact me via E-mail: `feolaezequiel@gmail.com`. 