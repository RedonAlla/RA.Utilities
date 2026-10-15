---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations.Extensions
```

The `HttpClientBuilderExtensions` class provides **extension members on [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder)** (declared inside an `extension(IHttpClientBuilder)` block) that attach [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)s to a typed client's message handler pipeline.
Chain them directly off [`AddHttpClientIntegration(...)`](./ServiceCollectionExtensions.md#addhttpclientintegration).

## `AddLoggingHandler()`

Adds the [`LoggingHandler`](../DelegatingHandler/LoggingHandler.md) to the pipeline, which logs the details of every outgoing request and its response as structured logs.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpClientBuilder` | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The builder to configure. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The original builder for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(
    builder.Configuration.GetSection("MyApi"))
    .AddLoggingHandler();
```

## `AddInternalHeadersForwardingHandler()`

Adds the [`InternalHeadersForwardHandler`](../DelegatingHandler/InternalHeadersForwardHandler.md) to the pipeline, which forwards the current request's `Authorization` token and `x-request-id` (or `TraceIdentifier` fallback) to every outgoing call — see the handler page for details.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpClientBuilder` | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The builder to configure. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The original builder for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(
    builder.Configuration.GetSection("MyApi"))
    .AddInternalHeadersForwardingHandler();
```

:::info[v10.2.0 rename]
These methods replace the v10.1.1 `WithHttpLoggingHandler()` and `WithInternalHeadersForwardingHandler()` extension methods from `DependencyInjectionExtensions`. The handler attachments are equivalent — `AddLoggingHandler()` attaches the handler formerly known as `RequestResponseLoggingHandler` (now [`LoggingHandler`](../DelegatingHandler/LoggingHandler.md)) — only the method names changed; see the [migration guide](../migration-guides).
:::
