---
title: DefaultResponsesOperationTransformer
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.OpenApi.OperationTransformers
```

The `DefaultResponsesOperationTransformer` is an [`IOpenApiOperationTransformer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.openapi.iopenapioperationtransformer) that automatically adds a standardized `500 Internal Server Error` response to every API operation in your OpenAPI document.

### 🎯 Purpose

The main goals of this transformer are:

1. **Standardize 500 Error Responses**: It ensures that every endpoint in your API documentation clearly and consistently shows that it can produce a structured `ErrorResponse` for an internal server error. It uses the `ErrorResponse` model from the `RA.Utilities.Api.Results` package to define the schema.
2. **Reduce Boilerplate Annotations**: It eliminates the need for developers to manually add a `[ProducesResponseType(typeof(ErrorResponse), 500)]` attribute to every single API endpoint. The transformer handles this globally.
3. **Operation-Level Granularity**: As an [`IOpenApiOperationTransformer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.openapi.iopenapioperationtransformer), it integrates perfectly into the OpenAPI generation pipeline at the operation level, making it a clean and efficient way to apply this specific rule.

In essence, while a `DocumentTransformer` is for broad, document-wide changes, an `OperationTransformer` like this one provides a scalpel for making precise, repeated changes to every endpoint.

### 🚀 Usage

To use the transformer, register it as an [`IOpenApiOperationTransformer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.openapi.iopenapioperationtransformer) in your `Program.cs`.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.DocumentTransformers;

var builder = WebApplication.CreateBuilder(args);

// ... other services

builder.Services.AddOpenApi(options =>
  options.AddOpenApiOperationTransformer<DefaultResponsesOperationTransformer>());

var app = builder.Build();

// ...
```

This ensures that every API endpoint documented in your Swagger/OpenAPI UI will correctly show the standard 500 error response, improving the clarity and completeness of your API contract with minimal effort.
