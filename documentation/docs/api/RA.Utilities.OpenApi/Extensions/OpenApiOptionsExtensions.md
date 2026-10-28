---
title: OpenApiOptionsExtensions
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.OpenApi.Extensions
```

The `OpenApiOptionsExtensions` class provides a set of convenient extension methods for `OpenApiOptions` to simplify the registration of the various document transformers and filters included in this package.

These helpers streamline the setup process in `Program.cs`, making the code cleaner and more readable.

:::info[v10.1.1 rename]
In v10.1.1 this class was renamed from `DependencyInjectionExtensions` to `OpenApiOptionsExtensions`, and the methods are now declared as C# 14 extension members on `OpenApiOptions`. Extension-method call sites are unchanged (`options.AddDefaultsDocumentTransformer()`, `options.AddTagOperationTransformer(...)`, …) — only code that references the class by name, such as `typeof(DependencyInjectionExtensions)` or a static call, needs updating.
:::

### `AddDefaultsDocumentTransformer()`

This is the recommended starting point. It registers a sensible default set of document transformers with a single call.

**What it does:**
*   **`DocumentInfoTransformer`**: Populates the API's title, version, description, summary, terms of service, contact, and license from `appsettings.json`.
*   **`BearerSecurityDocumentTransformer`**: Automatically adds the "Authorize" button and JWT security scheme if Bearer authentication is detected.
*   **`HeadersParameterTransformer`**: Adds common request and response headers (like `x-request-id`) to all API operations based on configuration.
*   **`HealthChecksDocumentTransformer`**: Documents the `GET /health` (JSON health report) and `GET /alive` (plain-text liveness) endpoints and adds the `Health` tag, without applying the Bearer security scheme to them.
*   **`ValueObjectSchemaTransformer`**: Documents the constraints of the value objects from `RA.Utilities.Core.ValueObjects` (`Email`, `Currency`, `SSN`, `IBAN`, and `Money`).
*   **`GroupTagsDocumentTransformer`**: Writes group-level documentation declared with `WithGroupSummary()` / `WithGroupDescription()` on a route group into the OpenAPI tags of the group.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddDefaultsDocumentTransformer();
});
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

### `AddValueObjectSchemaTransformer()`

This extension registers the [`ValueObjectSchemaTransformer`](../SchemaTransformers/ValueObjectSchemaTransformer.md), which documents the validation constraints enforced by the value objects from `RA.Utilities.Core.ValueObjects`. It is registered automatically by [`AddDefaultsDocumentTransformer()`](#adddefaultsdocumenttransformer); call it directly when you register schema transformers individually.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddValueObjectSchemaTransformer();
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
builder.Services.AddOpenApi(options =>
{
    options.AddDefaultResponsesOperationTransformer();
});
```

---

### `AddPolymorphismDocumentTransformer<T>(typesToInclude, discriminatorPropertyName = "Type")`

This extension registers the [`PolymorphismDocumentTransformer`](../DocumentTransformers/PolymorphismDocumentTransformer.md) to correctly document polymorphic types (a base class with several derived classes) using the `oneOf` and `discriminator` keywords.

#### Usage

You provide the base type `T` and a dictionary mapping discriminator values to the derived types.

```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddPolymorphismDocumentTransformer<Animal>(new()
    {
        { "Cat", typeof(Cat) },
        { "Dog", typeof(Dog) },
    });
});
```

---

### `AddTagOperationTransformer(IDictionary<string, string> tags)`

This extension registers the `TagOperationTransformer`, which enriches your OpenAPI documentation by adding or updating descriptions for tags.

**What it does:**
*   **Centralizes Documentation**: Allows you to define tag descriptions in `Program.cs` or configuration instead of scattering attributes across controllers.
*   **Enriches UI**: Populates the description field next to the tag name in the API reference UI.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddTagOperationTransformer(new Dictionary<string, string>
    {
        { "Products", "Operations related to product management." },
        { "Auth", "Authentication endpoints." }
    });
});
```

---

### `AddHealthChecksDocumentTransformer(IDictionary<string, string> tags)`

This extension is intended to register the [`HealthChecksDocumentTransformer`](../DocumentTransformers/HealthChecksDocumentTransformer.md), which documents the health check endpoints in the OpenAPI document.

:::caution[v10.1.0, still present in v10.3.0]
As implemented, this method registers `HeadersParameterTransformer` instead of `HealthChecksDocumentTransformer`, and the `tags` argument is not used. Use [`AddDefaultsDocumentTransformer()`](#adddefaultsdocumenttransformer) to document health check endpoints until this is corrected.
:::

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddHealthChecksDocumentTransformer(new Dictionary<string, string>
    {
        { "Health", "Service health check endpoints." }
    });
});
```

---

### `AddGroupTagsDocumentTransformer(IDictionary<string, string> tags)`

This extension registers the [`GroupTagsDocumentTransformer`](../OperationTransformers/GroupTagsDocumentTransformer.md), which writes group-level documentation declared with [`WithGroupSummary()` and `WithGroupDescription()`](./RouteGroupBuilderExtensions.md) into the OpenAPI tags that the group's endpoints carry.

:::caution[v10.3.0]
The `tags` argument is required by the current signature but is ignored. Group documentation is read from the route group's metadata, not from this dictionary.
:::

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    // The argument is required by the current signature but is not used.
    options.AddGroupTagsDocumentTransformer(new Dictionary<string, string>());
});
```

Prefer [`AddDefaultsDocumentTransformer()`](#adddefaultsdocumenttransformer), which already registers this transformer.

---

### Individual Transformer Extensions

For more granular control, you can also register each of the default transformers individually.

*   **`AddDocumentInfoTransformer()`**: Registers only the [`DocumentInfoTransformer`](../DocumentTransformers/DocumentInfoTransformer.md).
*   **`AddBearerSecurityDocumentTransformer()`**: Registers only the [`BearerSecurityDocumentTransformer`](../DocumentTransformers/BearerSecurityDocumentTransformer.md).
*   **`AddHeadersParameterTransformer()`**: Registers only the [`HeadersParameterTransformer`](../DocumentTransformers/HeadersParameterTransformer.md).
*   **`AddHealthChecksDocumentTransformer(tags)`**: Intended to register only the [`HealthChecksDocumentTransformer`](../DocumentTransformers/HealthChecksDocumentTransformer.md) — see the [caution](#addhealthchecksdocumenttransformeridictionarystring-string-tags) above.
*   **`AddGroupTagsDocumentTransformer(tags)`**: Registers only the [`GroupTagsDocumentTransformer`](../OperationTransformers/GroupTagsDocumentTransformer.md) — the `tags` argument is ignored; see the [caution](#addgrouptagsdocumenttransformeridictionarystring-string-tags) above.

#### Usage
```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentInfoTransformer();
    options.AddBearerSecurityDocumentTransformer();
    // ... and so on
});
```
