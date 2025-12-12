---
title: DefaultResponsesOperationTransformer
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `DefaultResponsesOperationTransformer` is an [`IOpenApiOperationTransformer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.openapi.iopenapioperationtransformer) that automatically adds a standardized `500 Internal Server Error` response to every API operation in your OpenAPI document.

### 🎯 Purpose

In any robust API, it's crucial to document that any endpoint could potentially fail with a server-side error. Manually adding `[ProducesResponseType(500, Type = typeof(ErrorResponse))]` to every single endpoint is repetitive and error-prone.

This transformer solves that problem by ensuring every operation consistently includes a documented `500` response, reflecting the "catch-all" error handling behavior common in web APIs. It serves as a more granular alternative to the `ResponsesDocumentTransformer`, which applies multiple error responses globally.

### ⚙️ How It Works

1.  **Runs Per-Operation**: As an [`IOpenApiOperationTransformer`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.openapi.iopenapioperationtransformer), it is executed once for every API endpoint (e.g., for `GET /users` and `POST /users` separately).
2.  **Checks for Existing Response**: It checks if the current operation already has a response defined for the `"500"` status code. If it does, the transformer does nothing to avoid overwriting custom definitions.
3.  **Adds the 500 Response**: If no `500` response is found, it adds one with:
    *   A standard description: `"Response designated 'catch-all' for any unexpected or unhandled exceptions..."`.
    *   A schema generated from the `Response<ErrorResponse>` type from the `RA.Utilities.Api.Results` package.
    *   A pre-defined JSON example of the error response body.

The final result in the OpenAPI JSON for an operation looks like this:

```json
"responses": {
  "200": {
    "description": "Success"
  },
  "500": {
    "description": "Response designated \"catch-all\" for any unexpected or unhandled exceptions that occur within your application",
    "content": {
      "application/json": {
        "schema": {
          "$ref": "#/components/schemas/ErrorResponse"
        },
        "examples": {
          "500": {
            "summary": "Default error response",
            "description": "This is an example of a default error response.",
            "value": {
              "isSuccess": false,
              "error": { "responseType": "Error", "message": "An unexpected error has occurred." }
            }
          }
        }
      }
    }
  }
}
```

### 🚀 Usage

To use the transformer, register it as an `IOpenApiOperationTransformer` in your `Program.cs`.

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

With this single line of configuration, every endpoint in your Swagger UI will now correctly display the `500` error response, improving the completeness of your API documentation.
