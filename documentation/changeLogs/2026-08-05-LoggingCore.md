---
title: RA.Utilities.Logging.Core
authors: [RedonAlla]
---

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
