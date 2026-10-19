---
title: RA.Utilities.Logging.Core
authors: [RedonAlla]
date: 2026-10-19
---

## Version 10.1.2
![Date Badge](https://img.shields.io/badge/Publish-19%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.1.2)

Maintenance release — no API, behavior, or dependency changes.

## Version 10.1.1
![Date Badge](https://img.shields.io/badge/Publish-15%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.1.1)

`RequestIdEnricher` is consolidated to a single `TraceId` property, `LoggingExtensions` is split into one class per extended type, and all extension classes are refactored to C# 14 extension members. Extension method names, signatures, and the namespace are unchanged.

<!-- truncate -->

### ⚠️ Breaking Changes

* **`RequestIdEnricher` now adds a single `TraceId` property**: the enricher no longer emits a separate request-id property (previously stored under the dashed name `x-request-id`), and it now writes `TraceId` unconditionally. The value is resolved from the `x-request-id` header first, then `HttpContext.TraceIdentifier`, then `Activity.Current` — so for requests that send the header, `TraceId` now holds the header value instead of `TraceIdentifier`, and events outside a request carry `TraceId: null` instead of omitting the property. **Migration**: query `TraceId` instead of the old request-id property, and exclude null values when grouping by trace id.

```json
  // v10.1.0
  { "x-request-id": "abc-123", "TraceId": "0HN7G3F2QK1L0:00000001" }

  // v10.1.1
  { "TraceId": "abc-123" }
```

* **`LoggingExtensions` split into `WebApplicationBuilderExtensions` and `LoggerEnrichmentConfigurationExtensions`**: `AddLoggingWithConfiguration()` (extension on `WebApplicationBuilder`) now lives in `WebApplicationBuilderExtensions`; `WithRequestIdEnricher()` (extension on `LoggerEnrichmentConfiguration`) lives in `LoggerEnrichmentConfigurationExtensions`. Both remain in the `RA.Utilities.Logging.Core.Extensions` namespace. Call sites need no changes — extension methods resolve by receiver type — but code that references the `LoggingExtensions` class name directly (`typeof(...)`, unit-test assertions, reflection) must be updated. **Migration**: see the [migration guide](https://redonalla.github.io/RA.Utilities/nuget-packages/Logging/RA.Utilities.Logging.Core/migration-guides).

```csharp
- var t = typeof(LoggingExtensions);
+ var b = typeof(WebApplicationBuilderExtensions);         // AddLoggingWithConfiguration()
+ var e = typeof(LoggerEnrichmentConfigurationExtensions); // WithRequestIdEnricher()
+ var a = typeof(ActivityExtensions);                      // GetActivityId()
```

### 📝 Improvements

* **C# 14 extension members**: `WebApplicationBuilderExtensions`, `LoggerEnrichmentConfigurationExtensions`, and `ActivityExtensions` now declare their members inside `extension(...)` blocks on `WebApplicationBuilder`, `LoggerEnrichmentConfiguration`, and `Activity`. The generated extension methods keep identical names and signatures, so the refactor is source-compatible for all callers.
* **One class per extended type**: the split mirrors the convention used across the RA.Utilities packages (`ServiceCollectionExtensions` / `ApplicationBuilderExtensions` / …), so each class has a single, obvious responsibility.
* **`GetActivityId()` receiver is non-nullable**: the C# 14 receiver is declared as `Activity`; call it with `?.` (`Activity.Current?.GetActivityId()`) to stay warning-free.
* **Documentation**: the Logging.Core API reference is split into one page per extension class (`WebApplicationBuilderExtensions`, `LoggerEnrichmentConfigurationExtensions`, `ActivityExtensions`), and a migration guide documents the new `TraceId`-only correlation model.

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-09%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/10.0.2)

Correlation moves from the log templates to the logging scope: `BaseHttpLogTemplate` no longer carries `RequestId` / `TraceIdentifier`, and request correlation is provided by the `X-Request-Id` log scope instead.

<!-- truncate -->

### ⚠️ Breaking Changes

* **`RequestId` and `TraceIdentifier` removed from `BaseHttpLogTemplate`**: the properties no longer exist on `BaseHttpLogTemplate` (and therefore not on `HttpRequestLogTemplate` / `HttpResponseLogTemplate` either). Code that sets or reads these properties will not compile.
  **Migration**: carry the correlation ID in a log scope — sinks like the Serilog `RequestIdEnricher` (`RA.Utilities.Logging.Core`) pick up `LoggingConstants.XRequestId` from the scope automatically.

```csharp
- var log = new HttpRequestLogTemplate { RequestId = requestId, TraceIdentifier = traceId, ... };

+ using (logger.BeginScope(new Dictionary<string, object> { [LoggingConstants.XRequestId] = requestId }))
+ {
+     logger.LogInformation("HTTP Request: {@RequestLog}", log);
+ }
```

### 📝 Improvements

* `BaseHttpLogTemplate` now only contains `Path`, `RequestedOn`, and `RemoteAddress`; `ToString()` still returns the JSON-serialized representation.

```csharp
  public abstract class BaseHttpLogTemplate
  {
-     public string? RequestId { get; set; }
-     public string? TraceIdentifier { get; set; }
      public string? Path { get; set; }
      public DateTime RequestedOn { get; set; } = DateTime.UtcNow;
      public string? RemoteAddress { get; set; }
  }
```


## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-27%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.1.0)

This release makes HTTP-context registration explicit when request ID enrichment is enabled, avoiding an implicit ASP.NET Core service registration for applications that do not use it.

<!-- truncate -->

### ⚠️ Breaking Changes

* **`IHttpContextAccessor` is no longer registered automatically**: `AddLoggingWithConfiguration` no longer registers `IHttpContextAccessor`. If you activate `RequestIdEnricher` through `appsettings.json` or `.Enrich.WithRequestIdEnricher()`, register it in your application:

  ```csharp
  builder.Services.AddHttpContextAccessor();
  builder.AddLoggingWithConfiguration();
  ```

### 📝 Improvements

* **Request ID enrichment remains opt-in**: Activate `RequestIdEnricher` through the Serilog `Enrich` configuration or the fluent `WithRequestIdEnricher` extension.
* **XML documentation clarified**: `AddLoggingWithConfiguration` now documents the explicit `IHttpContextAccessor` registration requirement for applications that enable request ID enrichment.

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-05%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.0.1)

This release fixes several bugs in the request ID enrichment pipeline, cleans up dead dependencies, and brings documentation in sync with the actual code.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **`ActivityExtensions.GetActivityId` now returns `string?`**: The return type changed from `string` to `string?`. When no `Activity` context is available, the method returns `null` instead of a random `Guid`. The `RequestIdEnricher` skips adding the `TraceId` property when the value is null — background and startup logs no longer receive unique random trace IDs per event. **Migration**: Update any code that assigns the result to a non-nullable `string` variable to use `string?` instead.

### 📝 Improvements

*   **`RequestIdEnricher` now resolves `HttpContext` lazily per event**: Previously the enricher captured `HttpContext` at construction time (always `null` outside a request), making the `x-request-id` header branch dead code. The enricher now stores the `IHttpContextAccessor` and resolves `HttpContext` on each `Enrich()` call, so request-scoped headers are correctly read.
*   **Removed unused `Serilog.Settings.AppSettings` dependency**: This legacy XML appSettings reader was not used by the code (configuration is read via `Serilog.Settings.Configuration`, included transitively through `Serilog.AspNetCore`).
*   **Fixed broken XML doc crefs**: Removed unresolvable cref references in `AddLoggingWithConfiguration`'s remarks and replaced them with plain-text descriptions.
*   **Sample config cleaned up**: `appsettings.serilog.json` no longer contains developer-specific absolute paths or application names.
*   **README updated**: Replaced all references to the non-existent `AddRaSerilog` method with the correct `AddLoggingWithConfiguration`. Rewrote "Key Features Configured by Default" to accurately reflect what the code configures vs. what is available via `appsettings.json`.

---

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.0.0)

Updated the project version from `10.0.0-rc.2` to the stable release version `10.0.0` in preparation for a production release.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.0.0-rc.2)

- Initial release of the core logging package.
- Provides `AddLoggingWithConfiguration` extension method for opinionated Serilog configuration.
- Includes request ID enrichment and exception details enrichment out of the box.
- Makes common Serilog sinks (Console, File, Async) and enrichers (Sensitive Data) available via `appsettings.json` configuration.
