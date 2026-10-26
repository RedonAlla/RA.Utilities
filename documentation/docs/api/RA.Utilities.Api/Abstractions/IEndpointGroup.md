---
title: IEndpointGroup
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Api.Abstractions
```

The `IEndpointGroup` interface represents a shared route group that [`IEndpoint`](./IEndpoint.md) implementations map their routes into.
Each group owns the route prefix, tags, versioning, and conventions for one feature of your API, and is created exactly once by the generated registration pipeline — before any endpoint is mapped.

## ⚙️ How It Works

### 1. Implement the Interface

You create a class for a feature (e.g., `ProductsGroup`) and implement the `IEndpointGroup` interface with two static members:

* `GroupName` — a unique name for the group. Every [`IEndpoint`](./IEndpoint.md) with the same `GroupName` is mapped into this group. Names must be unique across all groups in the assembly; duplicates are a compile-time error (`EPMG001`).
* `MapGroup` — receives the application's `IEndpointRouteBuilder` and the optional `ApiVersionSet` passed to `MapEndpoints`, and returns the configured `RouteGroupBuilder`. Apply the version set with `group.WithApiVersionSet(apiVersionSet)` to version every endpoint mapped into the group.

```csharp
// Features/Products/ProductsGroup.cs
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;

internal sealed class ProductsGroup : IEndpointGroup
{
    public static string GroupName => "Products";

    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app, ApiVersionSet? apiVersionSet = null)
    {
        RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}/products").WithTags("Products");

        // Applies the version set registered in Program.cs to every endpoint in the group.
        return apiVersionSet is null ? group : group.WithApiVersionSet(apiVersionSet);
    }
}
```

### 2. Map the Routes

After building the web application (`var app = builder.Build();`), you call the generated `app.MapEndpoints()` extension method.
The source generator that ships with this package discovers your `IEndpointGroup` implementations at compile time and invokes each `MapGroup` exactly once, storing the resulting builder under its `GroupName` before any endpoint is mapped.

```csharp
// Program.cs

using Asp.Versioning;
using Asp.Versioning.Builder;

var builder = WebApplication.CreateBuilder(args);

// Required when endpoints apply an ApiVersionSet.
builder.Services.AddApiVersioning();

var app = builder.Build();

// One version set, forwarded to every MapGroup and MapEndpoint implementation
ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

// Creates every group first, then maps every endpoint into its group
app.MapEndpoints(apiVersionSet);

app.Run();
```

Call `app.MapEndpoints()` without arguments when the API does not use versioning.
