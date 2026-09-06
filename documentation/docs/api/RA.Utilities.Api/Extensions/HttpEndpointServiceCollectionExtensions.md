```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `HttpEndpointServiceCollectionExtensions` class is the engine behind your **Endpoint Registration** feature.
Its purpose is to declutter your `Program.cs` file by automating the discovery and mapping of API endpoints.
This is a core pattern for implementing a clean Vertical Slice Architecture.

Unlike the previous reflection-based registration, the discovery happens entirely at **compile time**:

* The `RA.Utilities.Api.Generators` source generator (shipped inside this package as a build-time analyzer) scans your assembly for every [`IEndpointGroup`](../IEndpointGroup.md) and [`IEndpoint`](../IEndpoint.md) implementation and extends this partial class with the `MapEndpoints()` method.
* `MapEndpoints()` creates every group exactly once (before any endpoint is mapped) and then maps every endpoint into the group matching its `GroupName`.
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

  public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app)
    => app.MapGroup("api/products").WithTags("Products");
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

  public static void MapEndpoint(RouteGroupBuilder group)
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

var app = builder.Build();
// Maps all groups and endpoints discovered at compile time
// highlight-next-line
app.MapEndpoints();

app.Run();
```

This keeps your `Program.cs` clean and your endpoint definitions organized by feature.

## 🧭 Compile-Time Validation

Whenever the `GroupName` values are compile-time constants, the generator validates them at build time:

* `EPMG001` — two `IEndpointGroup` implementations declare the same `GroupName`.
* `EPMG002` — an `IEndpoint` references a `GroupName` no group declares.
* `EPMG003` — a generic endpoint/group type.
* `EPMG004` — a private or file-local endpoint/group type that the generated code cannot reference.
* `EPMG005` — static interface members implemented explicitly.

Names computed at runtime are resolved at startup; mismatches among those fail with a descriptive `InvalidOperationException` naming the endpoint type and the missing group key.
