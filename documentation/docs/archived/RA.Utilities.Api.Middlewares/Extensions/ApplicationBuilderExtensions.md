---
title: ApplicationBuilderExtensions
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Middlewares.Extensions
```

The `ApplicationBuilderExtensions` class provides a set of convenient "shortcut" methods for registering your custom middlewares with the ASP.NET Core application pipeline.
This is a common and recommended practice in the .NET ecosystem.

In ASP.NET Core, the standard way to add a middleware to the request pipeline is by calling `app.UseMiddleware<TMiddleware>()`.
While this works perfectly, it can be a bit generic and less descriptive.

The extension methods in this class, such as `UseHttpLoggingMiddleware()` and `UseDefaultHeadersMiddleware()`, wrap the `UseMiddleware<T>()` call in a more readable and discoverable function.


Instead of writing:
```csharp
app.UseMiddleware<HttpLoggingMiddleware>();
app.UseMiddleware<DefaultHeadersMiddleware>();
```

You can use the more fluent and discoverable extension methods:
```csharp
app.UseHttpLoggingMiddleware();
app.UseDefaultHeadersMiddleware();
```

## 🔑 Key Benefits:

1. **Readability**: Code in `Program.cs` becomes cleaner and more self-documenting.
`app.UseHttpLoggingMiddleware()` is more explicit about its intent than `app.UseMiddleware<HttpLoggingMiddleware>()`.
2. **Discoverability**: When a developer types `app.` in the editor, IntelliSense will suggest these descriptive method names, making it easier to find and use the middlewares provided by your library.
3. **Consistency**: It aligns with the pattern used by Microsoft and other popular .NET libraries for registering middleware, creating a consistent developer experience.

In short, this class is all about improving the developer experience by providing a fluent and intuitive API for pipeline configuration.

## 🧩 Available Extensions

### UseHttpLoggingMiddleware()

Registers the `HttpLoggingMiddleware` to the application's request pipeline.
This middleware is responsible for logging detailed information about HTTP requests and responses.

**Usage:**
```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var app = builder.Build();

// highlight-next-line
app.UseHttpLoggingMiddleware();

app.UseRouting();
// ...
```

### UseDefaultHeadersMiddleware()

Registers the `DefaultHeadersMiddleware` to the application's request pipeline.
This middleware enforces the presence of required headers, such as `X-Request-Id`.

**Usage:**
```csharp showLineNumbers
// Program.cs
using RA.Utilities.Api.Middlewares.Extensions;

var app = builder.Build();

app.UseRouting();

// highlight-next-line
app.UseDefaultHeadersMiddleware();

app.MapControllers();
// ...
```
