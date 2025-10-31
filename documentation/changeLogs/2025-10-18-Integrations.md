---
title: RA.Utilities.Logging.Shared
authors: [RedonAlla]
---

## Version 10.0.0-rc.2
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/10.0.0-rc.2)

This release modernizes the `RA.Utilities.Integrations` package, providing a robust and repeatable pattern for managing external API integrations.
It centralizes configuration, simplifies registration, and improves resilience with built-in retry policies.

<!-- truncate -->

### ✨ New Features

*   **Standardized Configuration**:
    *   Centralizes HTTP client settings (Base URL, timeouts, headers) in `appsettings.json` using the `HttpClientSettings` base class.

*   **Simplified Registration**:
    *   Introduced the `AddIntegrationHttpClient<TClient, TSettings>` extension method to register a typed `HttpClient`, bind its configuration, and apply default policies with a single line of code.

*   **Built-in Resilience**:
    *   Includes a default transient error handling policy (retry with exponential backoff) using Polly, improving the reliability of external API calls.

*   **Promotes Best Practices**:
    *   Encourages the use of typed `HttpClient`s via `IHttpClientFactory`, which provides better compile-time safety, intellisense, and connection management.

*   **Updated Documentation**:
    *   The `README.md` has been updated to provide a clear, step-by-step guide for setting up a typed client, from configuration to implementation.

### 🚀 Getting Started

Register your typed HTTP client in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register the typed client for "MyApi" integration
builder.Services.AddIntegrationHttpClient<IMyApiClient, MyApiClient, MyApiSettings>(
    builder.Configuration,
    configSection: "Integrations:MyApi"
);

var app = builder.Build();
```