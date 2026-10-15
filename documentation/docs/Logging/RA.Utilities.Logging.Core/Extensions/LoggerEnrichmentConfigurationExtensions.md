```bash
Namespace: RA.Utilities.Logging.Core.Extensions
```

The `LoggerEnrichmentConfigurationExtensions` class provides the fluent entry point for adding the [`RequestIdEnricher`](../Enrichers/index.md) to a Serilog pipeline.

The enricher attaches a single `TraceId` correlation property to every log event, which makes it possible to follow one operation across services. The value is resolved from the first source that yields one:

* **The `x-request-id` HTTP request header** — when the client or an upstream proxy sends one.
* **`HttpContext.TraceIdentifier`** — ASP.NET Core's identifier for a single request on a single server.
* **`Activity.Current`** — the fallback for background work, through [`ActivityExtensions.GetActivityId()`](./ActivityExtensions.md).

:::info v10.1.1
Before v10.1.1 this method was declared in a class named `LoggingExtensions`. It now lives in `LoggerEnrichmentConfigurationExtensions` (one class per extended type) and is written as a C# 14 extension member. The method name, signature, and namespace are unchanged — see the [migration guide](../migration-guides.mdx).
:::

:::note
`AddLoggingWithConfiguration()` does **not** call this method. Request ID enrichment is opt-in: either list `{ "Name": "WithRequestIdEnricher" }` in the `Serilog` → `Enrich` array, or call this extension when you build the `LoggerConfiguration` yourself.
:::

## Methods

### `WithRequestIdEnricher()`

Adds the [`RequestIdEnricher`](../Enrichers/index.md) to the enrichment configuration.

#### Parameters

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **enrich** | `LoggerEnrichmentConfiguration` | The Serilog enrichment configuration. This is the **receiver** of the extension member. |

#### Returns

| Type | Description |
| ---- | ----------- |
| `LoggerConfiguration` | The logger configuration, allowing further chaining of configuration methods. |

#### Exceptions

| Exception | Condition |
| --------- | --------- |
| `ArgumentNullException` | Thrown when `enrich` is `null`. |

#### Example

The method is typically used inside a manual Serilog setup. Register `IHttpContextAccessor` first so the enricher can read the current request:

```csharp showLineNumbers
using RA.Utilities.Logging.Core.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Required: the enricher reads the current request through IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    // highlight-next-line
    .Enrich.WithRequestIdEnricher() // Adds the custom enricher
    .WriteTo.Console()
    .CreateLogger();
```

When you rely on `AddLoggingWithConfiguration()` instead, activate the same enricher by name from `appsettings.json`:

```json showLineNumbers
{
  "Serilog": {
    "Enrich": [
      { "Name": "WithRequestIdEnricher" }
    ]
  }
}
```

:::info v10.1.1
Before v10.1.1 the enricher added a separate request-id property alongside `TraceId`. Since v10.1.1 the header value is folded into the single `TraceId` property, so `{TraceId}` in an output template shows the correlated value. See the [migration guide](../migration-guides.mdx) if your sinks query the old request-id property.
:::
