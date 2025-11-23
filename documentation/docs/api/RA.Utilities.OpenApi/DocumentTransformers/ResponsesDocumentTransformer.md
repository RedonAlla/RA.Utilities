---
title: ResponsesDocumentTransformer
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `ResponsesDocumentTransformer` is an `IOpenApiDocumentTransformer` that automatically adds standardized error responses to every operation in your OpenAPI document. It ensures your API contract accurately reflects the structured error models from the [`RA.Utilities.Api.Results`](../../RA.Utilities.Api.Results/index.mdx) package.

### 🎯 Purpose

The `ResponsesDocumentTransformer` is a powerful utility that automates the documentation of your API's standardized error responses.

In a well-designed API, you want to return consistent, structured error responses for common HTTP status codes like `400 Bad Request` or `404 Not Found`.
The [`RA.Utilities.Api.Results`](../../RA.Utilities.Api.Results/index.mdx) package provides exactly these models ([`BadRequestResponse`](../../RA.Utilities.Api.Results/BadRequestResponse.md), [`NotFoundResponse`](../../RA.Utilities.Api.Results/NotFoundResponse.md), etc.).

However, to make these visible in your OpenAPI (Swagger) documentation, you would normally have to decorate every single API endpoint with multiple `[ProducesResponseType]` attributes.
This is highly repetitive, clutters your controller code, and is easy to forget.

The ResponsesDocumentTransformer solves this problem elegantly.

### ⚙️ How It Works

1.  **Global Application**: It scans every API operation in the generated OpenAPI document.
2.  **Adds Standard Responses**: For each operation, it adds response definitions for the following HTTP status codes:
    *   `400 Bad Request`: Uses the schema for [`BadRequestResponse`](../../RA.Utilities.Api.Results/BadRequestResponse.md).
    *   `404 Not Found`: Uses the schema for [`NotFoundResponse`](../../RA.Utilities.Api.Results/NotFoundResponse.md).
    *   `409 Conflict`: Uses the schema for [`ConflictResponse`](../../RA.Utilities.Api.Results/ConflictResponse.md).
    *   `500 Internal Server Error`: Uses the schema for [`ErrorResponse`](../../RA.Utilities.Api.Results/ErrorResponse.md)..
3.  **Prevents Duplicates**: It is smart enough not to add a response if one for that status code has already been defined on the operation (e.g., via a `[ProducesResponseType]` attribute).
4. **Uses Correct Schemas**: It intelligently uses the schemas from the [`RA.Utilities.Api.Results`](../../RA.Utilities.Api.Results/index.mdx) models ([`BadRequestResponse`](../../RA.Utilities.Api.Results/BadRequestResponse.md), [`NotFoundResponse`](../../RA.Utilities.Api.Results/NotFoundResponse.md), [`ConflictResponse`](../../RA.Utilities.Api.Results/ConflictResponse.md), and [`ErrorResponse`](../../RA.Utilities.Api.Results/ErrorResponse.md)). This means your Swagger UI will show a precise JSON example of the error body for each status code.

### 🚀 Usage

This transformer is not included in the `AddDefaultsDocumentTransformer()` method and should be registered separately.

#### Register the Transformer in `Program.cs`

Call `AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>()` to add it to the pipeline.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.DocumentTransformers;
// highlight-next-line
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer(); // Registers Info, Bearer, and Headers transformers

// highlight-next-line
builder.Services.AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpenApi();

// ... your endpoints
```

With this transformer enabled, every endpoint in your Swagger UI will now show the correct, structured JSON response examples for all standard error codes, providing a complete and accurate API contract for your consumers.
