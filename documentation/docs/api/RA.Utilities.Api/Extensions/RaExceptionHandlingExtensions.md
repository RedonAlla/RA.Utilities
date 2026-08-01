```powershell
Namespace: RA.Utilities.Api.Extensions
```

The `RaExceptionHandlingExtensions` class is a key component of your `RA.Utilities.Api` package.
Its purpose is to provide a clean, discoverable way to register the [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) with your application, giving every unhandled exception a single, standardized error response.

The [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) implements the `IExceptionHandler` interface (introduced in .NET 8).
It acts as a centralized safety net: it catches unhandled exceptions, logs them, maps them to a structured error response via `ErrorResultResponse.Result`, and writes that response to the client.
Without these extensions, wiring it up requires two separate, easy-to-forget calls:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();
```

`RaExceptionHandlingExtensions` wraps both calls in expressive, discoverable extension methods:

**1. `AddRaExceptionHandling()`**:
This method registers the [`GlobalExceptionHandler`](../GlobalExceptionHandler.mdx) with the dependency injection (DI) container by calling `AddExceptionHandler<GlobalExceptionHandler>()`.
It should be called during service registration, on the `IServiceCollection` returned by `builder.Services`.

**2. `UseRaExceptionHandling()`**:
This method adds the exception handler middleware to the request pipeline by calling `UseExceptionHandler()`.
It should be called early in the pipeline, before other middleware, so it can catch exceptions thrown by subsequent middleware and endpoints.

Both methods return the same instance they were called on, so additional calls can be chained.

## 🚀 Usage Guide

```csharp showLineNumbers
// Program.cs

// highlight-next-line
using RA.Utilities.Api.Extensions;

WebApplicationBuilder builder =
  WebApplication.CreateBuilder(args);

// Registers the GlobalExceptionHandler with DI
// highlight-start
builder.Services
  .AddRaExceptionHandling();
// highlight-end

var app = builder.Build();

// Adds the exception handler middleware to the pipeline
// highlight-next-line
app.UseRaExceptionHandling();

app.Run();
```

That's it — one line for registration and one line for pipeline setup.
This keeps your `Program.cs` clean and guarantees that every unhandled exception is logged and converted into a consistent, structured error response.
