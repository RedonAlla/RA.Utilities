---
title: RA.Utilities.Integrations
authors: [RedonAlla]
---

## Version 1.0.0-preview.6.3
[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Integrations.svg)](https://www.nuget.org/packages/RA.Utilities.Integrations/)

This is the initial preview release of the `RA.Utilities.Integrations` package. It provides a standardized and resilient way to manage HTTP client integrations in .NET applications.

<!-- truncate -->

### ✨ New Features

*   **Initial Release**: Introduction of a comprehensive toolkit for external API integrations.
*   **Typed HttpClient Registration**:
    *   `AddHttpClientIntegration`: A powerful extension method to register typed `HttpClient`s, bind them to `appsettings.json` configuration, and apply resilience policies.
*   **Resilience with Polly**:
    *   Includes a default transient fault-handling policy (retry with exponential backoff) out of the box.
    *   Allows for custom Polly policies to be injected for advanced scenarios like Circuit Breakers.
*   **Common Delegating Handlers**:
    *   `RequestResponseLoggingHandler`: For detailed logging of outgoing requests and incoming responses.
    *   `InternalHeadersForwardHandler`: To propagate `Authorization` and `x-request-id` headers in service-to-service communication.
    *   `ApiKeyAuthenticationHandler`: For easily adding API key authentication to requests.
*   **Base HTTP Client**:
    *   `BaseHttpClient`: A reusable base class that simplifies common HTTP operations like GET, POST, and PUT, including serialization and deserialization.
