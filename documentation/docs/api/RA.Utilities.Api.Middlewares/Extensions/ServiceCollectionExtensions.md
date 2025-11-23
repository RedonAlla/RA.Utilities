---
title: ServiceCollectionExtensions
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Api.Middlewares.Extensions
```

The `ServiceCollectionExtensions` class provides extension methods for [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to simplify the registration of middleware services and their configurations in the ASP.NET Core dependency injection (DI) container.

### 🎯 Purpose

The `ServiceCollectionExtensions` class focused on providing a clean and simple developer experience for setting up the middlewares.
Its purpose is to encapsulate all the necessary Dependency Injection (DI) registrations required for the middlewares to function correctly.

In ASP.NET Core, before you can use a service or middleware that has its own dependencies or configuration, you must first register it with the built-in DI container.

The extension methods in this class—`AddHttpLoggingMiddleware()` and `AddDefaultHeadersMiddleware()`—serve as convenient, single-line shortcuts to handle this setup:

1. **Simplifies Configuration**: Instead of forcing the developer to know the inner workings of each middleware (e.g., that `HttpLoggingMiddleware` requires [`RecyclableMemoryStreamManager`](https://github.com/Microsoft/Microsoft.IO.RecyclableMemoryStream) as a singleton), it bundles all the necessary registrations into one easy-to-use method.

2. **Provides Optional Configuration**: Both methods accept an optional `Action<T>` delegate, allowing developers to easily customize the behavior of the middlewares (like ignoring certain paths) in a fluent and discoverable way.

3. **Enforces Best Practices**: By providing these extensions, the library guides users to follow the standard .NET convention for library setup, making the `Program.cs` file more readable and maintainable.

In short, this class abstracts away the boilerplate setup code, allowing developers to enable powerful middleware features with minimal effort and a greatly reduced chance of misconfiguration.

## 🧩 Available Extensions

### AddHttpLoggingMiddleware()

Registers all services required for the `HttpLoggingMiddleware`. This includes:
- The `HttpLoggingMiddleware` itself.
- The `RecyclableMemoryStreamManager` as a singleton for high-performance stream handling.
- Configuration for `HttpLoggingOptions`.

#### Usage:

You can call the method directly or provide an action to configure its options.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddHttpLoggingMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.MaxBodyLogLength = 8192; // 8 KB
});
// highlight-end
```

### AddDefaultHeadersMiddleware()

Registers the `DefaultHeadersMiddleware` and its associated `DefaultHeadersOptions` configuration.

#### Usage:

You can call the method directly or provide an action to configure its options, such as paths to exclude from header validation.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddDefaultHeadersMiddleware(options =>
{
    options.PathsToIgnore.Add("/swagger");
    options.PathsToIgnore.Add("/health");
});
// highlight-end
```
