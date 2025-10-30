# RA.Utilities.OpenApi

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.OpenApi?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.OpenApi/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities)](https://github.com/RedonAlla/RA.Utilities/blob/main/LICENSE)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.OpenApi.svg)](https://www.nuget.org/packages/RA.Utilities.OpenApi/)

A utility library to enhance and customize OpenAPI (Swagger) documentation in ASP.NET Core applications.

This package provides a collection of `IOpenApiDocumentTransformer` implementations to automate common modifications to your generated OpenAPI specification. Instead of manually annotating every endpoint with attributes for common headers or security schemes, you can apply these transformations globally.

The primary goals are to:
- **Reduce Boilerplate**: Automatically add common parameters (like correlation IDs) to all API operations.
- **Enforce Consistency**: Ensure that all endpoints consistently document required headers and responses.
- **Simplify Configuration**: Keep your endpoint definitions clean by centralizing OpenAPI modifications.

## Getting started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.OpenApi
```

Or through the NuGet Package Manager in Visual Studio.

---

## Default Transformers

This package includes a set of default transformers that can be registered with a single extension method, `AddDefaultsDocumentTransformer()`.

### `DocumentInfoTransformer`

This transformer populates the top-level information of your OpenAPI document (like title, version, description, and contact info) directly from your `appsettings.json` configuration.

**What it does:**

1.  **Reads from Configuration**: It uses `OpenApiInfoSettings` to bind to a configuration section (e.g., `OpenApi:Info`).
2.  **Populates `OpenApiInfo`**: It sets the `Title`, `Version`, `Description`, `Contact`, and `License` fields in the generated document.
3.  **Simplifies Customization**: Allows you to update your API's documentation details without changing any code.

### `BearerSecuritySchemeTransformer`

This transformer automatically adds a "Bearer" security scheme to your OpenAPI document if it detects that JWT Bearer authentication is registered in your application. This enables the "Authorize" button in the Swagger UI, allowing users to test protected endpoints.

**What it does:**

1.  **Detects JWT Authentication**: It checks if an authentication scheme named `Bearer` or `BearerToken` is present.
2.  **Adds Security Scheme**: If detected, it adds the standard JWT Bearer security definition to the document's components.
3.  **Reduces Boilerplate**: Eliminates the need to manually configure `AddSecurityDefinition` and `AddSecurityRequirement` in `SwaggerGenOptions` for this common scenario.

### `HeadersParameterTransformer`

This document transformer automatically adds common correlation and tracing headers to every operation in your OpenAPI document. This is useful for microservices and distributed systems where tracking requests is essential.

**What it does:**

1.  **Adds Request Headers**: It adds headers like `x-request-id` as parameters to all API requests. This documents the need for clients to provide a unique identifier for each call.
2.  **Adds Response Headers**: It adds headers like `x-request-id` and `trace-id` to all possible responses for every operation. This informs clients that they can expect these headers back for logging and debugging.

Both the request and response headers are configurable via `HeadersParameterSettings`.

### `ResponsesDocumentTransformer`

This transformer automatically adds standardized OpenAPI responses for common HTTP status codes like `400`, `404`, `409`, and `500`. It uses the response models from `RA.Utilities.Api.Results` to generate the schema for these error responses.

**What it does:**

1.  **Standardizes Error Responses**: Ensures that your OpenAPI documentation accurately reflects the structured error responses (`BadRequestResponse`, `NotFoundResponse`, `ConflictResponse`, `ErrorResponse`) that your API produces.
2.  **Reduces Annotations**: Eliminates the need to manually add `[ProducesResponseType]` attributes for these common errors on every single endpoint.

---

## Configuration

The transformers are configured via `appsettings.json`.
Below are the details for the available settings classes.

### `OpenApiInfoSettings`

Used by `DocumentInfoTransformer` to populate the document's `info` object.

**`appsettings.json` Section:** `OpenApi:Info`

| Property           | Type                               | Description                                                      |
| ------------------ | ---------------------------------- | ---------------------------------------------------------------- |
| **Title**          | `string`                           | The title of the API.                                            |
| **Version**        | `string`                           | The version of the API.                                          |
| **Description**    | `string`                           | A short description of the API.                                  |
| **TermsOfService** | `string` (URI)                     | A URL to the Terms of Service for the API.                       |
| **Contact**        | `OpenApiContactSettings` (object)  | The contact information for the exposed API. See details below.  |
| **License**        | `OpenApiLicenseSettings` (object)  | The license information for the exposed API. See details below.  |

#### `OpenApiContactSettings`

| Property  | Type           | Description                               |
| --------- | -------------- | ----------------------------------------- |
| **Name**  | `string`       | The identifying name of the contact person/organization. |
| **Email** | `string`       | The email address of the contact person/organization. |
| **Url**   | `string` (URI) | The URL pointing to the contact information. |

#### `OpenApiLicenseSettings`

| Property | Type           | Description                            |
| -------- | -------------- | -------------------------------------- |
| Name     | `string`       | The license name used for the API.     |
| Url      | `string` (URI) | A URL to the license used for the API. |

### `HeadersParameterSettings`

Used by `HeadersParameterTransformer` to add common headers to requests and responses.

**`appsettings.json` Section:** `OpenApiHeaders`

| Property          | Type                     | Description                                                                 |
| ----------------- | ------------------------ | --------------------------------------------------------------------------- |
| RequestHeaders    | `List<[HeaderDefinition](#eaderdefinition)>` | A list of header definitions to add to all API requests.                    |
| ResponseHeaders   | `List<[HeaderDefinition](#eaderdefinition)>` | A list of header definitions to add to all API responses.                   |

#### `HeaderDefinition`

Represents a single header to be added to the OpenAPI specification.

| Property      | Type      | Default      | Description                                                                  |
| ------------- | --------- | ------------ | ---------------------------------------------------------------------------- |
| Name          | `string`  | `""`         | The name of the header (e.g., "x-request-id").                               |
| Description   | `string`  | `""`         | A description of the header's purpose.                                       |
| Required      | `bool`    | `true`       | Specifies if the header is required.                                         |
| Type          | `JsonSchemaType`  | `String`   | The schema type for the header value (e.g., "String", "Integer").            |
| Format        | `string`  | `"uuid"`     | The format of the header value (e.g., "uuid", "date-time").                  |
| Value         | `object`  | `null`       | An example value for the header.                                             |

### Example `appsettings.json`

```json
{
  "OpenApiInfoSettings": {
    "Info": {
      "Title": "My Awesome API",
      "Version": "v1.0.0",
      "Summary": "A brief and catchy summary of the API.",
      "Description": "This is a more detailed description of the API. It can include **Markdown** for rich text formatting, explaining what the API does, its main features, and how to get started.",
      "TermsOfService": "https://example.com/terms",
      "Contact": {
        "Name": "API Support Team",
        "Url": "https://example.com/support",
        "Email": "support@example.com"
      },
      "License": {
        "Name": "MIT License",
        "Url": "https://opensource.org/licenses/MIT"
      }
    }
  }
}
```

## 🔗 Dependencies

-   [`Microsoft.AspNetCore.OpenApi`](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi)
-   [`Microsoft.Extensions.Options.ConfigurationExtensions`](https://www.nuget.org/packages/microsoft.extensions.options.configurationextensions/)


## Usage

### Recommended: Using Default Transformers

The easiest way to get started is by using the `AddDefaultsDocumentTransformer` extension method.
This single call registers `DocumentInfoTransformer`, `BearerSecuritySchemeTransformer`, and `HeadersParameterTransformer`.

You can then register other transformers, like `ResponsesDocumentTransformer`, individually.

```csharp
// Program.cs

using RA.Utilities.OpenApi.DocumentTransformers;
using RA.Utilities.OpenApi.Extensions;
using RA.Utilities.OpenApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// (Optional) Configure settings from appsettings.json for the transformers.
builder.Services.Configure<OpenApiInfoSettings>(builder.Configuration.GetSection(OpenApiInfoSettings.AppSettingsKey));
builder.Services.Configure<HeadersParameterSettings>(builder.Configuration.GetSection(HeadersParameterSettings.AppSettingsKey));

// 1. Add the OpenApi services and register the default transformers.
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();

// 2. Register any additional transformers.
builder.Services.AddOpenApiDocumentTransformer<ResponsesDocumentTransformer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. Map the OpenAPI document endpoints.
app.MapOpenApi();

app.MapGet("/weatherforecast", () => "Hello World!")
   .WithName("GetWeatherForecast")
   .WithOpenApi();

app.Run();
```

With this transformer enabled, your Swagger UI will now show predefined response schemas for 400, 404, 409, and 500 status codes on all endpoints, matching the structure of your `RA.Utilities.Api.Results` models.


## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/api/OpenApi/).

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch. Please use a descriptive name (e.g., `feature/add-security-transformer` or `fix/readme-typo`).
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

- Follow the existing coding style and conventions used in the project.
- Ensure all public members are documented with clear XML comments.
- Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!