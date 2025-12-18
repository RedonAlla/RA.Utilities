---
title: DependencyInjectionExtensions
sidebar_position: 6
---

```bash
Namespace: RA.Utilities.OpenApi.Extensions
```

The `DependencyInjectionExtensions` class provides a set of convenient extension methods for `OpenApiOptions` to simplify the registration of the various document transformers and filters included in this package.

These helpers streamline the setup process in `Program.cs`, making the code cleaner and more readable.

### `AddDefaultsDocumentTransformer()`

This is the recommended starting point. It registers a sensible default set of document transformers with a single call.

**What it does:**
*   **`DocumentInfoTransformer`**: Populates the API's title, version, and description from `appsettings.json`.
*   **`BearerSecurityDocumentTransformer`**: Automatically adds the "Authorize" button and JWT security scheme if Bearer authentication is detected.
*   **`HeadersParameterTransformer`**: Adds common request and response headers (like `x-request-id`) to all API operations based on configuration.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();
```

---

### `AddFluentValidationRules()`

This extension registers the [`FluentValidationSchemaTransformer`](../SchemaTransformers/FluentValidationSchemaTransformer.md), which automatically enriches the OpenAPI schema with constraints derived from your `FluentValidation` rules.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddFluentValidationRules();
});
```

---

### `AddEnumXmlDescriptionTransformer(string xmlPath)`

This extension registers the [`EnumXmlSchemaTransformer`](../SchemaTransformers/EnumXmlSchemaTransformer.md), which reads your project's XML documentation file to add descriptive markdown tables to your enum schemas.

#### Usage
```csharp
// In Program.cs
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddOpenApi(options =>
{
    options.AddEnumXmlDescriptionTransformer(xmlPath);
});
```

---

### `AddDefaultResponsesOperationTransformer()`

This extension registers the [`DefaultResponsesOperationTransformer`](../OperationTransformers/DefaultResponsesOperationTransformer.md), which automatically adds a standardized `500 Internal Server Error` response to every API operation.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi()
    .AddDefaultResponsesOperationTransformer();
```

---

### `AddPolymorphismDocumentTransformer<T>()`

This extension registers the [`PolymorphismDocumentTransformer`](../DocumentTransformers/PolymorphismDocumentTransformer.md) to correctly document polymorphic types (a base class with several derived classes) using the `oneOf` and `discriminator` keywords.

#### Usage

You provide the base type `T` and a dictionary mapping discriminator values to the derived types.

```csharp
// In Program.cs
builder.Services.AddOpenApi()
    .AddPolymorphismDocumentTransformer<ErrorResponse>(new()
    {
        { "NotFound", typeof(NotFoundResponse) },
        { "Conflict", typeof(ConflictResponse) },
    });
```

---

### Individual Transformer Extensions

For more granular control, you can also register each of the default transformers individually.

*   **`AddDocumentInfoTransformer()`**: Registers only the [`DocumentInfoTransformer`](../DocumentTransformers/DocumentInfoTransformer.md).
*   **`AddBearerSecurityDocumentTransformer()`**: Registers only the [`BearerSecurityDocumentTransformer`](../DocumentTransformers/BearerSecurityDocumentTransformer.md).
*   **`AddHeadersParameterTransformer()`**: Registers only the [`HeadersParameterTransformer`](../DocumentTransformers/HeadersParameterTransformer.md).

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi()
    .AddDocumentInfoTransformer()
    .AddBearerSecurityDocumentTransformer();
    // ... and so on
```
