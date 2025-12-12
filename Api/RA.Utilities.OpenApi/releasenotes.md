# Release Notes for RA.Utilities.OpenApi

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-12%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.1)

Add `DefaultResponsesOperationTransformer` and `PolymorphismSchemaFilter` for enhanced OpenAPI document transformation.

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.0)

Updated the project version from `10.0.0-rc.2` to `10.0.0`, indicating a transition from release candidate to official stable release. This change suggests that the OpenAPI utilities have reached a level of stability and feature completeness deemed ready for production use


## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.0-rc.2)

This release of `RA.Utilities.OpenApi` provides a robust set of tools to enhance and customize OpenAPI (Swagger) documentation in ASP.NET Core applications. It automates common modifications to your generated OpenAPI specification, reducing boilerplate and enforcing consistency.

### ✨ Key Features

*   **`DocumentInfoTransformer`**: Populates the top-level `info` object of your OpenAPI document (title, version, description, etc.) directly from your `appsettings.json` configuration, allowing you to update documentation details without code changes.

*   **`BearerSecuritySchemeTransformer`**: Automatically adds a "Bearer" security scheme to your OpenAPI document when JWT authentication is detected, enabling the "Authorize" button in Swagger UI for testing protected endpoints.

*   **`HeadersParameterTransformer`**: Adds common, configurable headers (like `x-request-id`) to every API operation, ensuring consistent documentation for tracing and correlation in distributed systems.

*   **`ResponsesDocumentTransformer`**: Automatically adds standardized OpenAPI responses for common HTTP status codes (`400`, `404`, `409`, `500`), using schema models from `RA.Utilities.Api.Results` to ensure your error contracts are clearly documented.

*   **Simplified Setup**: Includes an `AddDefaultsDocumentTransformer()` extension method to register the most common transformers (`DocumentInfo`, `BearerSecurityScheme`, and `HeadersParameter`) with a single line of code.

*   **Configuration-Driven**: All transformers are configurable via `appsettings.json` using strongly-typed settings classes (`OpenApiInfoSettings`, `HeadersParameterSettings`), keeping your application code clean and focused.

### 🚀 Getting Started

1.  **Configure Settings**: Add `OpenApiInfoSettings` and `OpenApiHeaders` sections to your `appsettings.json`.
2.  **Register Services**: In `Program.cs`, configure the settings classes:
    ```csharp
    builder.Services.Configure<OpenApiInfoSettings>(builder.Configuration.GetSection(OpenApiInfoSettings.AppSettingsKey));
    builder.Services.Configure<HeadersParameterSettings>(builder.Configuration.GetSection("OpenApiHeaders"));
    ```
3.  **Add Transformers**: Use the extension methods to register the transformers:
    ```csharp
    builder.Services.AddOpenApi()
        .AddDefaultsDocumentTransformer(); // Adds Info, Bearer, and Headers transformers
    
    // Register other transformers individually
    builder.Services.AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>();
    ```
4.  **Map Endpoints**: Finally, map the OpenAPI endpoints in your request pipeline:
    ```csharp
    app.MapOpenApi();
    ```
