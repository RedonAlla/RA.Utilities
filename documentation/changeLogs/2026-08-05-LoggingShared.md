---
title: RA.Utilities.Logging.Shared
authors: [RedonAlla]
---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-05%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/10.0.1)

This release fixes naming inconsistencies, wires up the previously unused `RequestId` property, and corrects the `RequestedOn` timestamp behavior.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **`Schema` renamed to `Scheme`**: The property on `HttpRequestLogTemplate` was misspelled. Consumers that reference `Schema` must update to `Scheme`. All built-in middleware and delegating handler consumers in the RA.Utilities ecosystem have been updated.
*   **`Duration` type changed from `string` to `double?`**: `HttpResponseLogTemplate.Duration` is now a nullable double (milliseconds) instead of a formatted string. Consumers should assign the raw numeric value (e.g., `duration.TotalMilliseconds`) rather than a pre-formatted string like `"X.XX ms"`.

### ✨ New Features

*   **`RequestId` is now populated from the `x-request-id` header**: All built-in consumers (`HttpLoggingMiddleware` in both Api and Api.Middlewares, `RequestResponseLoggingHandler` in Integrations) now read the `x-request-id` header and populate `RequestId` on the log templates. This enables end-to-end request correlation across services.

### 📝 Improvements

*   **`RequestedOn` is now a settable property**: Previously it was a computed property (`=> DateTime.UtcNow`) evaluated at serialization time, producing different timestamps for request vs. response logs. It now defaults to `DateTime.UtcNow` at construction and can be set to the actual request time.
*   **`LoggingConstants` class added**: A new `LoggingConstants.XRequestId` constant provides the canonical header name (`"x-request-id"`) used across all logging packages.
*   **README updated**: Fixed the broken `RequestId` table row, added missing `Host` and `Duration` properties, updated `Schema` → `Scheme`, fixed the "HTTP Request body" copy-paste error on the response model, and corrected `StatusCode` to show `int?`.

---

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/10.0.0)

Updated the project version from `10.0.0-rc.2` to the stable release version `10.0.0` in preparation for a production release.

## Version 10.0.0-rc.2

![Date Badge](https://img.shields.io/badge/Publish-18%20October%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/10.0.0-rc.2)

- Initial release of the shared logging package.
- Provides core models for structured HTTP request and response logging (`HttpRequestLogTemplate`, `HttpResponseLogTemplate`, `BaseHttpLogTemplate`).
- Acts as a foundational dependency for other logging packages like `RA.Utilities.Api.Middlewares`.
