# Release Notes for RA.Utilities.OpenApi

[#e93b779](https://github.com/RedonAlla/RA.Utilities/commit/e93b77985552f1e5400573e9aa792a0d78c996c2) Update version to `10.0.6` and correct example serialization in `OpenApiOperationUtilities`

## Version 10.0.5
![Date Badge](https://img.shields.io/badge/Publish-23%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.5-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.5)

### ✨ New Features

This release enhances the `OpenApiOperationUtilities` class by introducing two new methods for adding multiple examples to an OpenAPI operation at once. These helpers streamline the process of documenting various request or response scenarios, making your API documentation more comprehensive.

- **`AddRequestExamples(...)`**: Adds multiple request examples to the specified OpenApi operation.
- **`AddResponseExamples(...)`**: Adds multiple response examples to the specified OpenApi operation.

---

## Version 10.0.3
![Date Badge](https://img.shields.io/badge/Publish-117%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.3)

### 🐞 Bug fix

The extension method for registering the polymorphism transformer has been fixed and improved.
It now correctly accepts parameters for configuration.

| Parameter | Description |
| --------- | ----------- |
| `T` (generic type parameter) | The base type for polymorphism. The schema name is derived from this type (e.g., `nameof(T)`). |
| `typesToInclude` | A dictionary mapping schema names to their corresponding types to include in the polymorphic schema. |
| `discriminatorPropertyName` | The name of the discriminator property. ***Defaults to "Type".*** |

**Before:**
```csharp
public static OpenApiOptions AddPolymorphismSchemaFilter(this OpenApiOptions options) =>
        options.AddDocumentTransformer<PolymorphismSchemaFilter>();
```
**now:**
```csharp
    public static OpenApiOptions AddPolymorphismDocumentTransformer<T>(this OpenApiOptions options, Dictionary<string, Type> typesToInclude, string discriminatorPropertyName = "Type") =>
        options.AddDocumentTransformer(new PolymorphismDocumentTransformer(nameof(T), typesToInclude, discriminatorPropertyName));
```

### 💥 Breaking changes

#### 🏗️ Architectural Changes
The following transformers have been made internal to enforce a consistent registration pattern:
- `BearerSecurityDocumentTransformer`
- `DocumentInfoTransformer`
- `HeadersParameterTransformer`
- `PolymorphismDocumentTransformer`
- `DefaultResponsesOperationTransformer`

:::info

They must now be registered using their corresponding extension methods provided in `DependencyInjectionExtensions`.
This change simplifies setup and reduces the chance of misconfiguration.

:::

#### 🔏 Renaming
* `BearerSecuritySchemeTransformer` => `BearerSecurityDocumentTransformer`
* `PolymorphismSchemaFilter` => `PolymorphismDocumentTransformer`

Changing: `BearerSecurityDocumentTransformer`, `DocumentInfoTransformer`, `HeadersParameterTransformer`, `PolymorphismDocumentTransformer`
`DefaultResponsesOperationTransformer` each mean you now can only those thought extensions method inside `DependencyInjectionExtensions`

---

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-117%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.2)

### ✨ New Features

#### OpenAPI Schema Enrichment from FluentValidation Rules

You can now automatically reflect your `FluentValidation` rules in your OpenAPI schema. The new `FluentValidationSchemaTransformer` inspects your validators and applies corresponding constraints to your model properties.

#### Key Benefits:

* **Single Source of Truth**: Your validation rules defined in C# are now the single source of truth, and your API documentation will always stay in sync.
* **Improved API Usability**: API consumers can see constraints like required fields, length limits, and patterns directly in the documentation, leading to fewer invalid requests.

#### Supported Rules:

* `NotNull` / `NotEmpty` -> Marks the property as required.
* `Length` / `MinimumLength` / `MaximumLength` -> Sets `minLength` and `maxLength`.
* `Matches` (Regular Expression) -> Sets the `pattern`.
* `GreaterThan` / `GreaterThanOrEqualTo` / `LessThan` / `LessThanOrEqualTo` -> Sets `minimum`, `maximum`, `exclusiveMinimum`, and `exclusiveMaximum`.
* `InclusiveBetween` / `ExclusiveBetween` -> Sets `minimum`, `maximum`, and the corresponding exclusive flags.
* `EmailAddress` -> Sets the format to `email`.
* `CreditCard` -> Sets the format to `credit-card`.

**How to use it**: Simply call the `AddFluentValidationRules()` extension method when configuring your OpenAPI services.

```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    // ... other configurations
    options.AddFluentValidationRules();
});
```

#### OpenAPI Enum Descriptions from XML Documentation
The new `EnumXmlSchemaTransformer` enhances your enums in the OpenAPI schema by appending a markdown table with the descriptions from your XML documentation comments.

#### Key Benefits:

* **Clearer Enum Definitions**: Provides clear, human-readable descriptions for each enum value directly in the API documentation.
* **Reduces Ambiguity**: Helps consumers understand the meaning of each enum value without having to look at the source code.

**Example Output**: If you have an enum with XML comments, the transformer will append a table like this to the schema's description:

| Value	| Description |
| ----- | ----------- |
| Value1 | This is the first value. |
| Value2 | This is the second value. |

**How to use it**: First, ensure your project is configured to generate an XML documentation file. Then, use the `AddEnumXmlDescriptionTransformer()` extension method, providing the path to the XML file.

```csharp
// In Program.cs
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddOpenApi(options =>
{
    // ... other configurations
    options.AddEnumXmlDescriptionTransformer(xmlPath);
});
```

---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-12%20December%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.OpenApi/10.0.1)

Add `DefaultResponsesOperationTransformer` and `PolymorphismDocumentTransformer` for enhanced OpenAPI document transformation.

### `DefaultResponsesOperationTransformer`

This operation transformer automatically adds standardized OpenAPI responses for common HTTP status codes for  `500`.
It uses the response models from `RA.Utilities.Api.Results` to generate the schema for these error responses.

This transformer is an `IOpenApiOperationTransformer`, meaning it applies to each API operation individually.
It is a more targeted alternative to the `BearerSecurityDocumentTransformer`.

**What it does:**

1.  **Standardizes Error Responses**: Ensures that your OpenAPI documentation accurately reflects the structured error responses (`ErrorResponse`) that your API produces.
2.  **Reduces Annotations**: Eliminates the need to manually add `[ProducesResponseType]` attributes for these common errors on every single endpoint.
3.  **Operation-Level Granularity**: As an `IOpenApiOperationTransformer`, it integrates seamlessly with Swashbuckle's operation-level processing pipeline.

### `PolymorphismDocumentTransformer`

This schema filter enhances the OpenAPI documentation for APIs that use polymorphic types (i.e., base classes with multiple derived classes). It correctly represents the inheritance structure in the generated schema, making it understandable for clients and tools like NSwag or AutoRest.

**What it does:**

1.  **Identifies Polymorphic Types**: It detects when a base type is used in an API and finds all its derived types within the loaded assemblies.
2.  **Adds `oneOf` Schema**: It modifies the base type's schema to use the `oneOf` keyword, listing all the derived types. This allows API clients to understand that the response or request body can be one of several concrete implementations.
3.  **Enables Code Generation**: Fixes issues with client-side code generation where polymorphic types would otherwise be ignored or misinterpreted.

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

*   **`BearerSecurityDocumentTransformer`**: Automatically adds standardized OpenAPI responses for common HTTP status codes (`400`, `404`, `409`, `500`), using schema models from `RA.Utilities.Api.Results` to ensure your error contracts are clearly documented.

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
    builder.Services.AddOpenApiDocumentTransformer<BearerSecurityDocumentTransformer>();
    ```
4.  **Map Endpoints**: Finally, map the OpenAPI endpoints in your request pipeline:
    ```csharp
    app.MapOpenApi();
    ```
