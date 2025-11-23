```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `EndpointExtensions` class is a key component of your `RA.Utilities.Api` package.
Its purpose is to provide a clean, discoverable, and scalable pattern for organizing API endpoints, which is a core tenet of Vertical Slice Architecture.

Instead of defining all API routes in a single, potentially massive `Program.cs` file, this class allows you to group related endpoints into their own dedicated files.
It achieves this through a simple two-step process powered by two extension methods:

**1. `AddEndpoints()`**:
This method scans a given assembly for any classes that implement the `IEndpoint` interface.
It then registers each of these classes with the dependency injection (DI) container.
This step makes the application aware of all the available endpoint definitions.

**2. `MapEndpoints()`**:
After the application is built, this method retrieves all the `IEndpoint` implementations that were registered in the previous step.
It then iterates through them and calls their respective MapEndpoint method, which effectively adds the routes to the ASP.NET Core routing table.

By using these extensions, you can keep your `Program.cs` file clean and focused on application configuration, while your endpoint definitions are neatly organized by feature.

## 🚀 Usage Guide

### Step 1: Create an `IEndpoint` implementation

```csharp showLineNumbers
// Features/Products/ProductEndpoints.cs

// highlight-next-line
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

### Step 2: Register the endpoints in `Program.cs`

The `MapEndpoints` method scans the specified assembly (or the calling assembly by default) for all types implementing `IEndpoint` and calls their `MapEndpoints` method.

```csharp
// Program.cs

WebApplicationBuilder builder =
  WebApplication.CreateBuilder(args);
// highlight-start
builder.Services
  .AddEndpoints(Assembly.GetExecutingAssembly());
// highlight-end

var app = builder.Build();
// Scans the assembly and registers all IEndpoint implementations
// highlight-next-line
app.MapEndpoints();

app.Run();
```

This keeps your `Program.cs` clean and your endpoint definitions organized by feature.