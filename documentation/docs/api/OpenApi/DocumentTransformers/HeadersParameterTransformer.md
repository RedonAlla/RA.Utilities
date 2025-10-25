---
title: HeadersParameterTransformer
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `HeadersParameterTransformer` is an `IOpenApiDocumentTransformer` that automatically adds common headers to every operation in your OpenAPI document.
This is essential for consistently documenting cross-cutting concerns like distributed tracing (`x-request-id`).

### 🎯 Purpose

The `HeadersParameterTransformer` is a powerful utility that automates the documentation of common HTTP headers across all of your API endpoints.
In modern applications, especially in distributed systems, it's standard practice to include headers for cross-cutting concerns like traceability (`x-request-id`) or tenancy.

Manually adding these headers to the OpenAPI (Swagger) documentation for every single endpoint is repetitive, error-prone, and clutters your controller code with attributes.
This transformer solves that problem.

### ⚙️ How It Works

1. **Configuration-Driven**: It reads a list of desired headers from your `appsettings.json` file via the [`HeadersParameterSettings`](../Configuration/HeadersParameterSettings.md) class.
You can define separate lists for request headers and response headers.
2. **Global Application**: It iterates through every single API operation defined in your OpenAPI document.
3. **Adds Request Headers**: For each operation, it adds the configured request headers (e.g., `x-request-id`) to the list of parameters.
This makes it clear to clients that these headers are expected.
4. **Adds Response Headers**: It also adds the configured response headers to every possible response status code (`200`, `404`, `500`, etc.) for each operation.
This informs clients what headers they can expect to receive back from the API.

By using this transformer, you ensure your API's contract is consistently and accurately documented, promoting best practices without adding any boilerplate to your application code.

### 🚀 Usage

The recommended way to use this transformer is by registering the default set of transformers with the `AddDefaultsDocumentTransformer()` extension method. This method includes the `HeadersParameterTransformer` automatically.

#### Step 1: Configure Headers in `appsettings.json`

Define the headers you want to add in your `appsettings.json`.

```json showLineNumbers
// appsettings.json
{
  "OpenApiHeaders": {
    "RequestHeaders": [
      {
        "Name": "x-request-id",
        "Description": "A unique identifier for the API call, used for tracing.",
        "Required": true,
        "Type": "String",
        "Format": "uuid"
      }
    ],
    "ResponseHeaders": [
      {
        "Name": "x-request-id",
        "Description": "The unique identifier of the request, echoed back for correlation."
      }
    ]
  }
}
```

#### Step 2: Register Services in `Program.cs`

Configure the settings and add the default transformers.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;
using RA.Utilities.OpenApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
// Bind the settings from appsettings.json
builder.Services.Configure<HeadersParameterSettings>(
    builder.Configuration.GetSection(HeadersParameterSettings.AppSettingsKey)
);

// Add the OpenApi services and register the default transformers
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();
// highlight-end

var app = builder.Build();

// ...

app.MapOpenApi();
```

With this setup, every endpoint in your Swagger UI will now correctly document that it requires an `x-request-id` header in the request and will return it in the response.
