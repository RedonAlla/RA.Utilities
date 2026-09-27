# Release Notes

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
