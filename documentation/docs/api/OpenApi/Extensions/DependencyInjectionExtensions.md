---
title: DependencyInjectionExtensions
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.OpenApi.Extensions
```

The `DependencyInjectionExtensions` class provides a convenient extension method for `OpenApiOptions` to simplify the registration of the document transformers included in this package.

### 🎯 Purpose

The `DependencyInjectionExtensions` class in the `RA.Utilities.OpenApi` package provides a convenient "shortcut" method to simplify the setup of OpenAPI documentation in your ASP.NET Core application.

Its primary purpose is to bundle the registration of several common and highly useful `IOpenApiDocumentTransformer` implementations into a single, easy-to-use extension method: `AddDefaultsDocumentTransformer()`.

## 🧩 Available Extensions

### AddDefaultsDocumentTransformer()

This extension method registers a default set of `IOpenApiDocumentTransformer` implementations.
It is the recommended starting point for configuring `RA.Utilities.OpenApi`.

The following transformers are registered by this method:

1.  **`DocumentInfoTransformer`**: Populates the API's title, version, and description from `appsettings.json`.
2.  **`BearerSecuritySchemeTransformer`**: Automatically adds the "Authorize" button and JWT security scheme if Bearer authentication is detected.
3.  **`HeadersParameterTransformer`**: Adds common request and response headers (like `x-request-id`) to all API operations based on configuration.

Instead of registering each transformer individually in your Program.cs like this:

```csharp showLineNumbers
builder.Services.AddOpenApi()
    .AddDocumentTransformer<DocumentInfoTransformer>()
    .AddDocumentTransformer<BearerSecuritySchemeTransformer>()
    .AddDocumentTransformer<HeadersParameterTransformer>();
```

You can achieve the same result with a single, more readable line of code:

```csharp showLineNumbers
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();
```

This approach has several benefits:

1. **Reduces Boilerplate**: It significantly cleans up your `Program.cs` file.
2. **Promotes Convention**: It provides a sensible set of default configurations, guiding developers toward best practices for API documentation.
3. **Improves Discoverability**: It makes it easy to apply a standard set of documentation rules to any project using this library.


#### 🚀 Usage

Call `AddDefaultsDocumentTransformer()` after `AddOpenApi()` in your `Program.cs`.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;
using RA.Utilities.OpenApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// (Optional) Bind settings from appsettings.json for the transformers
builder.Services.Configure<OpenApiInfoSettings>(
    builder.Configuration.GetSection(OpenApiInfoSettings.AppSettingsKey)
);
builder.Services.Configure<HeadersParameterSettings>(
    builder.Configuration.GetSection(HeadersParameterSettings.AppSettingsKey)
);

// highlight-start
// Add OpenApi services and register the default transformers
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();
// highlight-end

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpenApi();
// ...
```
