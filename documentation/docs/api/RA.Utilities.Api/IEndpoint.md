---
title: IEndpoint
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Api.Abstractions
```

The primary purpose of the `IEndpoint` interface is to organize and declutter API endpoint registration in an ASP.NET Core application.
As an API grows, defining all the routes directly in the `Program.cs` file can make it messy and difficult to maintain.

The `IEndpoint` interface introduces a clean, discoverable pattern to solve this problem by allowing you to keep each endpoint in its own feature-focused file, mapped into the shared route group identified by its [`IEndpointGroup`](./IEndpointGroup.md).

## ⚙️ How It Works

The workflow is straightforward:

### 1. Implement the Interface

You create a class for a specific endpoint (e.g., `GetProductsEndpoint`) and implement the `IEndpoint` interface with two static members:

* `GroupName` — the name of the [`IEndpointGroup`](./IEndpointGroup.md) this endpoint belongs to.
* `MapEndpoint` — receives the group's `RouteGroupBuilder` and defines the routes, just as you would in `Program.cs`.

```csharp
// Features/Products/GetProductsEndpoint.cs
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

internal sealed class GetProductsEndpoint : IEndpoint
{
    public static string GroupName => "Products";

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/", () =>
        {
            // Logic to get all products
            return Results.Ok("All products");
        });

        group.MapGet("/{id}", (int id) =>
        {
            // Logic to get a single product
            return Results.Ok($"Product {id}");
        });
    }
}
```

### 2. Map the Routes

After building the web application (`var app = builder.Build();`), you call the generated `app.MapEndpoints()` extension method.
The source generator that ships with this package discovers your `IEndpoint` implementations at compile time and invokes each `MapEndpoint` with the `RouteGroupBuilder` of the group matching its `GroupName`.

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Maps all groups and endpoints discovered at compile time
app.MapEndpoints();

app.Run();
```

By following this pattern, you keep your `Program.cs` file clean and maintainable, while your endpoint definitions remain neatly organized by feature.

## 🧭 Compile-Time Validation

When the `GroupName` is a compile-time constant, the generator validates it against the discovered groups at build time:

* `EPMG001` — two `IEndpointGroup` implementations declare the same `GroupName`.
* `EPMG002` — an `IEndpoint` references a `GroupName` no group declares.

Names computed at runtime (e.g. `typeof(X).Name`) are resolved at startup instead; a mismatch then throws an `InvalidOperationException` naming the endpoint type and the missing group key.
