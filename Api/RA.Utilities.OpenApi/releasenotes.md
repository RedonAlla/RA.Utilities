# Release Notes for RA.Utilities.OpenApi

## Version 1.0.0-preview.6.3

This is the initial release of `RA.Utilities.OpenApi`, a utility library designed to enhance and customize OpenAPI (Swagger) documentation in ASP.NET Core applications. It automates common modifications to your generated OpenAPI specification, reducing boilerplate and enforcing consistency.

### ✨ Key Features

*   **`DocumentInfoTransformer`**: Populates the top-level `info` object of your OpenAPI document (title, version, description) directly from your `appsettings.json` configuration.

*   **`BearerSecuritySchemeTransformer`**: Automatically adds a "Bearer" security scheme to your OpenAPI document, enabling the "Authorize" button in Swagger UI when JWT authentication is used.

*   **`HeadersParameterTransformer`**: Adds common headers (like `x-request-id`) to every operation in your OpenAPI document, ensuring consistent documentation for tracing and correlation.

*   **`ResponsesDocumentTransformer`**: Automatically adds standardized OpenAPI responses for common HTTP status codes (`400`, `404`, `409`, `500`), using the schema models from `RA.Utilities.Api.Results`.

*   **Simplified Setup**: Includes an `AddDefaultsDocumentTransformer()` extension method to register the most common transformers with a single line of code.

*   **Configuration-Driven**: All transformers are configurable via `appsettings.json` using strongly-typed settings classes (`OpenApiInfoSettings`, `HeadersParameterSettings`), keeping your code clean.

### 🚀 Getting Started

1.  **Configure Settings**: Add `OpenApi:Info` and `OpenApiHeaders` sections to your `appsettings.json`.
2.  **Register Services**: In `Program.cs`, configure the settings classes:
    ```csharp
    builder.Services.Configure<OpenApiInfoSettings>(builder.Configuration.GetSection("OpenApi:Info"));
    builder.Services.Configure<HeadersParameterSettings>(builder.Configuration.GetSection("OpenApiHeaders"));
    ```
3.  **Add Transformers**: Use the extension methods to register the transformers:
    ```csharp
    builder.Services.AddOpenApi()
        .AddDefaultsDocumentTransformer(); // Adds Info, Bearer, and Headers transformers
    builder.Services.AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>();
    ```
4.  **Map Endpoints**: Finally, map the OpenAPI endpoints in your pipeline: `app.MapOpenApi()`.
