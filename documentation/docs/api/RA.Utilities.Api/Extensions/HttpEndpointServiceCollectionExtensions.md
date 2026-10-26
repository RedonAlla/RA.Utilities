```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `HttpEndpointServiceCollectionExtensions` class is the engine behind your **Endpoint Registration** feature.
Its purpose is to declutter your `Program.cs` file by automating the discovery and mapping of API endpoints.
This is a core pattern for implementing a clean Vertical Slice Architecture.

Unlike the previous reflection-based registration, the discovery happens entirely at **compile time**:

* The `RA.Utilities.Api.Generators` source generator (shipped inside this package as a build-time analyzer) scans your assembly for every [`IEndpointGroup`](../Abstractions/IEndpointGroup.md) and [`IEndpoint`](../Abstractions/IEndpoint.md) implementation and extends this partial class with the `MapEndpoints()` method.
* `MapEndpoints(ApiVersionSet? apiVersionSet = null)` creates every group exactly once (before any endpoint is mapped) and then maps every endpoint into the group matching its `GroupName`, forwarding the optional `ApiVersionSet` to every `MapGroup` and `MapEndpoint` implementation.
* No reflection, no DI scanning, and no manual registration list.

## 🚀 Usage Guide

### Step 1: Create an `IEndpointGroup` implementation

```csharp showLineNumbers
// Features/Products/ProductsGroup.cs

// highlight-next-line
using RA.Utilities.Api.Abstractions;

internal sealed class ProductsGroup : IEndpointGroup
{
  public static string GroupName => "Products";

  public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app, ApiVersionSet? apiVersionSet = null)
  {
    RouteGroupBuilder group = app.MapGroup("api/v{version:apiVersion}/products").WithTags("Products");
    return apiVersionSet is null ? group : group.WithApiVersionSet(apiVersionSet);
  }
}
```

### Step 2: Create the `IEndpoint` implementations

```csharp showLineNumbers
// Features/Products/GetProductsEndpoint.cs

// highlight-next-line
using RA.Utilities.Api.Abstractions;

internal sealed class GetProductsEndpoint : IEndpoint
{
  public static string GroupName => "Products";

  public static void MapEndpoint(RouteGroupBuilder group, ApiVersionSet? apiVersionSet = null)
  {
    group.MapGet("/", () =>
    {
      // Logic to get all products
      return Results.Ok("All products");
    });
  }
}
```

### Step 3: Call `MapEndpoints()` in `Program.cs`

```csharp showLineNumbers
// Program.cs

WebApplicationBuilder builder =
  WebApplication.CreateBuilder(args);

// Required when endpoints apply an ApiVersionSet.
builder.Services.AddApiVersioning();

var app = builder.Build();

// Maps all groups and endpoints discovered at compile time.
// The optional ApiVersionSet is forwarded to every MapGroup and MapEndpoint implementation.
// highlight-next-line
app.MapEndpoints(app.NewApiVersionSet()
  .HasApiVersion(new ApiVersion(1))
  .ReportApiVersions()
  .Build());

app.Run();
```

Call `app.MapEndpoints()` without arguments when the API does not use versioning.

This keeps your `Program.cs` clean and your endpoint definitions organized by feature.

## 🧭 Compile-Time Validation

Whenever the `GroupName` values are compile-time constants, the generator validates them at build time:

* `EPMG001` — two `IEndpointGroup` implementations declare the same `GroupName`.
* `EPMG002` — an `IEndpoint` references a `GroupName` no group declares.
* `EPMG003` — a generic endpoint/group type.
* `EPMG004` — a private or file-local endpoint/group type that the generated code cannot reference.
* `EPMG005` — static interface members implemented explicitly.

Names computed at runtime are resolved at startup; mismatches among those fail with a descriptive `InvalidOperationException` naming the endpoint type and the missing group key.
