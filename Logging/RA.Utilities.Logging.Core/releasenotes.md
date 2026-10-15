# Release Notes

## Version 10.1.1
![Date Badge](https://img.shields.io/badge/Publish-15%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.1.1)

`RequestIdEnricher` is consolidated to a single `TraceId` property, `LoggingExtensions` is split into one class per extended type, and all extension classes are refactored to C# 14 extension members. Extension method names, signatures, and the namespace are unchanged.

### ⚠️ Breaking Changes

* **`RequestIdEnricher` now adds a single `TraceId` property**: the enricher no longer emits a separate request-id property (previously stored under the dashed name `x-request-id`), and it now writes `TraceId` unconditionally. The value is resolved from the `x-request-id` header first, then `HttpContext.TraceIdentifier`, then `Activity.Current` — so for requests that send the header, `TraceId` now holds the header value instead of `TraceIdentifier`, and events outside a request carry `TraceId: null` instead of omitting the property.

  ```json
  // v10.1.0
  { "x-request-id": "abc-123", "TraceId": "0HN7G3F2QK1L0:00000001" }

  // v10.1.1
  { "TraceId": "abc-123" }
  ```

  **Migration**: query `TraceId` instead of the old request-id property, and exclude null values when grouping by trace id.

* **`LoggingExtensions` split into `WebApplicationBuilderExtensions` and `LoggerEnrichmentConfigurationExtensions`**: `AddLoggingWithConfiguration()` (extension on `WebApplicationBuilder`) now lives in `WebApplicationBuilderExtensions`; `WithRequestIdEnricher()` (extension on `LoggerEnrichmentConfiguration`) lives in `LoggerEnrichmentConfigurationExtensions`. Both remain in the `RA.Utilities.Logging.Core.Extensions` namespace. Call sites need no changes — extension methods resolve by receiver type — but code that references the `LoggingExtensions` class name directly (`typeof(...)`, unit-test assertions, reflection) must be updated:

  ```csharp
  // v10.1.0 and earlier
  typeof(LoggingExtensions)

  // v10.1.1
  typeof(WebApplicationBuilderExtensions)            // AddLoggingWithConfiguration()
  typeof(LoggerEnrichmentConfigurationExtensions)    // WithRequestIdEnricher()
  typeof(ActivityExtensions)                         // GetActivityId()
  ```

### 📝 Improvements

* **C# 14 extension members**: `WebApplicationBuilderExtensions`, `LoggerEnrichmentConfigurationExtensions`, and `ActivityExtensions` now declare their members inside `extension(...)` blocks on `WebApplicationBuilder`, `LoggerEnrichmentConfiguration`, and `Activity`. The generated extension methods keep identical names and signatures, so the refactor is source-compatible for all callers.
* **One class per extended type**: the split mirrors the convention used across the RA.Utilities packages, so each class has a single, obvious responsibility.
* **`GetActivityId()` receiver is non-nullable**: the C# 14 receiver is declared as `Activity`; call it with `?.` (`Activity.Current?.GetActivityId()`) to stay warning-free.

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-27%20September%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.1.0)

This release makes HTTP-context registration explicit when request ID enrichment is enabled, avoiding an implicit ASP.NET Core service registration for applications that do not use it.

### ⚠️ Breaking Changes

* **`IHttpContextAccessor` is no longer registered automatically**: `AddLoggingWithConfiguration` no longer registers `IHttpContextAccessor`. If you activate `RequestIdEnricher` through `appsettings.json` or `.Enrich.WithRequestIdEnricher()`, register it in your application:

  ```csharp
  builder.Services.AddHttpContextAccessor();
  builder.AddLoggingWithConfiguration();
  ```

### 📝 Improvements

* **Request ID enrichment remains opt-in**: `RequestIdEnricher` is activated only through the Serilog `Enrich` configuration or the fluent `WithRequestIdEnricher` extension.
* **XML documentation clarified**: `AddLoggingWithConfiguration` now documents the explicit `IHttpContextAccessor` registration requirement for applications that enable request ID enrichment.

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-05%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Core/10.0.1)

This release fixes several bugs in the request ID enrichment pipeline, cleans up dead dependencies, and brings documentation in sync with the actual code.

### ⚠️ Breaking Changes

* **`ActivityExtensions.GetActivityId` now returns `string?`**: The return type changed from `string` to `string?`. When no `Activity` context is available, the method returns `null` instead of a random `Guid`. The `RequestIdEnricher` skips adding the `TraceId` property when the value is null, so background and startup logs no longer receive unique random trace IDs per event. Update any code that assigns the result to a non-nullable `string` variable to use `string?` instead.

### 📝 Improvements

* **`RequestIdEnricher` now resolves `HttpContext` lazily per event**: The enricher resolves the current context during `Enrich()`, so request-scoped headers are read correctly.
* **Removed unused `Serilog.Settings.AppSettings` dependency**: Configuration is read through `Serilog.Settings.Configuration`, which is included transitively through `Serilog.AspNetCore`.
* **Fixed broken XML documentation references**: `AddLoggingWithConfiguration` remarks no longer contain unresolvable cref references.
* **Cleaned the sample configuration**: `appsettings.serilog.json` no longer contains developer-specific absolute paths or application names.
* **Updated the README**: References to the non-existent `AddRaSerilog` method now use `AddLoggingWithConfiguration`.

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
