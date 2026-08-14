---
title: RA.Utilities.Integrations
authors: [RedonAlla]
---

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-14%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/10.1.0)

This release overhauls the `BaseHttpClient` API with support for all four HTTP verbs and strongly-typed query string and header parameters, backed by a new Roslyn incremental source generator.

<!-- truncate -->

### ✨ New Features

*   **Full HTTP Verb Coverage**: `BaseHttpClient` now provides `GetAsync`, `PostAsync`, `PutAsync` and `DeleteAsync` — each with a typed-response overload and a string overload — covering the complete CRUD surface for external APIs.

*   **Strongly-Typed Query String Parameters**: Mark a `partial` class with `[QueryParameters]` and the source generator implements `IQueryStringRequest` for it, mapping each public property to a query string key-value pair. Null values are skipped, `bool` values are formatted as `true`/`false`, `DateTime`/`DateTimeOffset` values use the round-trip format, and collections are emitted as repeated keys. Override the emitted key per property with `[QueryParameterName("...")]`.

*   **Strongly-Typed Header Parameters**: Mark a `partial` class with `[HeaderParameters]` and the generator implements the new `IHeaderRequest` contract, mapping each public property to an HTTP header. Override the header name per property with `[HeaderParameterName("...")]`, e.g. `x-request-id`.

*   **Compile-Time Code Generation**: A new incremental Roslyn source generator, shipped inside the package as an analyzer, generates the parameter mappings at compile time — giving full IntelliSense and compile-time safety, with zero reflection.

*   **Null-Safe Requests**: Query and header parameters are optional; passing `null` builds a plain request instead of throwing.

*   **JSON Request Bodies**: `PostAsync` and `PutAsync` serialize request bodies as JSON (`application/json; charset=utf-8`) with camel-case property naming.

*   **Query String Utilities**: The query string building logic is consolidated into the new `QueryUtilities` class; the existing `DictionaryExtensions.ToQueryString(...)` extension is kept as a backward-compatible delegate.

### ⚠️ Breaking Changes

*   `IIntegrationSettings` no longer carries `MediaType`/`Encoding`; JSON is now the fixed content type.
*   `BaseHttpClient` methods now accept `IHeaderRequest?` instead of `Dictionary<string, string>?` for headers.
*   Typed-response overloads now return `Task<TResponse?>` and yield `null` for empty response bodies.
*   `AddOptionWithValidations(...)` was removed from `DependencyInjectionExtensions`; use `AddHttpClientIntegration` for validated integration settings.
*   See the [Migration Guide](/RA.Utilities/nuget-packages/Integrations/migration-guides) for detailed before/after examples.

### 🚀 Getting Started

```csharp
using RA.Utilities.Integrations.Attributes;

[QueryParameters]
public partial class GetProductsQuery
{
    [QueryParameterName("category_id")]
    public int? CategoryId { get; init; }
}

[HeaderParameters]
public partial class RequestHeaders
{
    [HeaderParameterName("x-request-id")]
    public string? XCorrelationId { get; init; }
}

Product? product = await client.GetAsync<Product>(
    "products",
    new GetProductsQuery { CategoryId = 5 },
    new RequestHeaders { XCorrelationId = "trace-1" });

await client.PostAsync<Product, Product>("products", product);
await client.PutAsync<Product, Product>($"products/{product.Id}", product);
await client.DeleteAsync("products/1");
```

---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-24%20April%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/10.0.1)

This release focuses on internal dependency updates and version synchronization within the RA.Utilities ecosystem.

### ✨ Improvements

*   **Ecosystem Alignment**: Updated internal project references to `RA.Utilities.Core.Constants` and `RA.Utilities.Logging.Shared` to version 10.0.1.
*   **Consistency**: Synchronized the package version to maintain parity with the core components following the introduction of new semantic exceptions.

---

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/10.0.0)

Updated package version `10.0.9-rc` (release candidate) to `10.0.0`, indicating a transition to a stable release.
This change signifies that the project is no longer in the release candidate phase and is considered ready for production use, reflecting confidence in its stability and completeness.

<!-- truncate -->

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/10.0.0-rc.2)

This release modernizes the `RA.Utilities.Integrations` package, providing a robust and repeatable pattern for managing external API integrations.
It centralizes configuration, simplifies registration, and improves resilience with built-in retry policies.

### ✨ New Features

*   **Standardized Configuration**: Centralizes HTTP client settings (Base URL, timeouts, headers) in `appsettings.json` using the `HttpClientSettings` base class.
*   **Simplified Registration**: Introduced the `AddIntegrationHttpClient<TClient, TSettings>` extension method to register a typed `HttpClient`, bind its configuration, and apply default policies with a single line of code.
*   **Built-in Resilience**: Includes a default transient error handling policy (retry with exponential backoff) using Polly, improving the reliability of external API calls.
*   **Promotes Best Practices**: Encourages the use of typed `HttpClient`s via `IHttpClientFactory`, which provides better compile-time safety, intellisense, and connection management.
*   **Updated Documentation**: The `README.md` has been updated to provide a clear, step-by-step guide for setting up a typed client, from configuration to implementation.
