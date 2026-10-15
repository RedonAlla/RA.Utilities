---
title: RequestIdEnricher
---

```bash
Namespace: RA.Utilities.Logging.Core.Enrichers
Implements: Serilog.Core.ILogEventEnricher
```

The `RequestIdEnricher` class is a custom **Serilog enricher** that attaches a request correlation / trace identifier to every log event.

Its purpose is to make logs traceable and debuggable, especially in a microservices or distributed environment. When a single user action results in calls across multiple services, a consistent id lets you filter and find all related log entries across every system.

## ⚙️ How it works

`RequestIdEnricher` implements the single `ILogEventEnricher.Enrich(LogEvent, ILogEventPropertyFactory)` method, which Serilog calls for every event. It contributes one property: `TraceId`.

### Resolving the value

The value comes from the first source that yields one, in this order:

1. **The `x-request-id` HTTP request header** — a correlation id sent by the client or an upstream proxy. This is the preferred source, because a value assigned at the edge stays stable as the request travels through multiple services.
2. **`HttpContext.TraceIdentifier`** — ASP.NET Core's unique identifier for a single request on a single server.
3. **`Activity.Current`** — through [`ActivityExtensions.GetActivityId()`](../Extensions/ActivityExtensions.md), for background work that has no HTTP request.

If none of the three sources produces a value, the `TraceId` property is still added, with a `null` value. The package never invents a random identifier, so unrelated events cannot be mistaken for part of the same trace.

### How the value is written

The property is written with `AddPropertyIfAbsent`, so a `TraceId` already supplied by a logging scope is never overwritten. The `HttpContext` is resolved lazily inside `Enrich()`, through the stored `IHttpContextAccessor` — the enricher does not capture the context at construction time, so it works correctly regardless of when the logger is configured.

## Properties Used

| Property Name | Source |
| ------------- | ------ |
| `TraceId` | `x-request-id` HTTP request header, otherwise `HttpContext.TraceIdentifier`, otherwise `Activity.Current?.GetActivityId()` |

:::info v10.1.1 — single `TraceId` property
Before v10.1.1 the enricher added two properties: a request-id property sourced from the `x-request-id` header, plus a separate `TraceId`. Since v10.1.1 the header value is folded into the single `TraceId` property. If you query, index, or alert on the old request-id property in your sinks, see the [migration guide](../migration-guides.mdx).
:::

## Constructors

| Constructor | When it is used |
| ----------- | --------------- |
| `RequestIdEnricher()` | Uses a new `HttpContextAccessor`. This is the constructor Serilog calls when the enricher is activated by name from `appsettings.json` or through `WithRequestIdEnricher()`. |
| `RequestIdEnricher(IHttpContextAccessor contextAccessor)` | Uses the supplied accessor, typically resolved from the dependency-injection container. Use this when you construct the enricher yourself. |

## Dependency

The enricher depends on `IHttpContextAccessor` to read the current request. Register it in your application with `builder.Services.AddHttpContextAccessor()` when you activate the enricher.

`HttpContextAccessor` stores the current context in a process-wide `AsyncLocal`, so the enricher's own accessor instance observes the same request as the rest of the application — but only while an `IHttpContextAccessor` is registered for ASP.NET Core to populate. Without an HTTP context, the enricher still resolves a value from `Activity.Current` when one is available.

## Example

```csharp showLineNumbers
using RA.Utilities.Logging.Core.Enrichers;
using Serilog;

// Usually registered through configuration (WithRequestIdEnricher) rather than by hand.
// Construct it directly only when you need to supply your own IHttpContextAccessor.
var enricher = new RequestIdEnricher(httpContextAccessor);

Log.Logger = new LoggerConfiguration()
    .Enrich.With(enricher)
    .WriteTo.Console(
        outputTemplate: "{Timestamp:G} [{Level}] ({TraceId}) {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```
