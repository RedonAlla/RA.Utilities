---
title: RA.Utilities.Api.Results
authors: [RedonAlla]
---

## Version 10.0.3
![Date Badge](https://img.shields.io/badge/Publish-02%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.3-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Api.Results/10.0.3)

This release marks the final version of `RA.Utilities.Api.Results`. The package is now deprecated — all response model types have been moved to [RA.Utilities.Api](https://www.nuget.org/packages/RA.Utilities.Api/).

<!-- truncate -->

### ⚠️ Deprecation Notice

*   **Package Deprecated**: This package is deprecated and has been moved to [RA.Utilities.Api](https://www.nuget.org/packages/RA.Utilities.Api/). All response model types are now available in the `RA.Utilities.Api.Results` namespace within the `RA.Utilities.Api` package. Please migrate to `RA.Utilities.Api` for future updates.

### ✨ New Features & Improvements

*   **New Response Types**: Added dedicated response types for previously uncovered HTTP scenarios:
    *   `TooManyRequestsResponse`: Standardized 429 Too Many Requests response.
    *   `ServiceUnavailableResponse`: Standardized 503 Service Unavailable response.
    *   `GatewayTimeoutResponse`: Standardized 504 Gateway Timeout response.

*   **Consistency Improvements**:
    *   All response types are now `sealed` for consistency.
    *   `Response<T>` properties now use `init`-only accessors to prevent mutation after construction.
    *   Constructor parameter names standardized (`responseCode`/`responseMessage`) across all error response types.

*   **Refined Documentation**: Updated the `README.md` with comprehensive property tables and JSON payload examples for all response types, including the new additions.
