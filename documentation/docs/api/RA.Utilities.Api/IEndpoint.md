---
title: IEndpoint
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Api.Abstractions
```

The primary purpose of the `IEndpoint` interface is to organize and declutter API endpoint registration in an ASP.NET Core application.
As an API grows, defining all the routes directly in the `Program.cs` file can make it messy and difficult to maintain.

The `IEndpoint` interface introduces a clean, discoverable pattern to solve this problem by allowing you to group related endpoints into separate, feature-focused files.

## ⚙️ How It Works

The workflow is straightforward and consists of a few steps:

### 1. Implement the Interface:
You create a class for a specific feature (e.g., `ProductEndpoints`) and implement the `IEndpoint` interface.
Inside the required `MapEndpoint` method, you define all the routes for that feature, just as you would in `Program.cs.`

```csharp
// Features/Products/ProductEndpoints.cs
using RA.Utilities.Api.Abstractions;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", () =>
        {
            // Logic to get all products
            return Results.Ok("All products");
        });

        app.MapGet("/products/{id}", (int id) => 
        {
            // Logic to get a single product
            return Results.Ok($"Product {id}");
        });
    }
}
```

### 2. Discover and Register Endpoints:
In your `Program.cs`, you call the `builder.Services.AddEndpoints()` extension method.
This method scans your project's assembly for all classes that implement `IEndpoint` and registers them with the dependency injection container.

### 3. Map the Routes:
After building the web application (`var app = builder.Build();`), you call the `app.MapEndpoints()` extension method.
This method retrieves all the registered `IEndpoint` services and executes their `MapEndpoint` method, effectively adding all the organized routes to the application.

Here is the corresponding `Program.cs` setup:

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// Scans the assembly and registers all IEndpoint implementations with DI
builder.Services.AddEndpoints();

var app = builder.Build();

// Executes the MapEndpoint method on all registered implementations
app.MapEndpoints();

app.Run();
```

By following this pattern, you keep your `Program.cs` file clean and maintainable, while your endpoint definitions remain neatly organized by feature.