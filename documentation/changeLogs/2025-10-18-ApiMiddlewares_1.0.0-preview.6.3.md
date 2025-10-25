---
title: RA.Utilities.Api.Middlewares
authors: [RedonAlla]
---

## Version 1.0.0-preview.6.3
[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Api.Middlewares.svg)](https://www.nuget.org/packages/RA.Utilities.Api.Middlewares/)

This release focuses on significantly improving the documentation and clarifying the purpose and usage of the middlewares provided in this package. The goal is to make it easier for developers to implement robust logging and header validation in their APIs.

<!-- truncate -->

### ✨ New Features & Improvements

*   **Comprehensive Documentation**: The `README.md` has been completely updated to provide clear, step-by-step instructions for using both `HttpLoggingMiddleware` and `DefaultHeadersMiddleware`.
*   **Clarified Purpose**: The documentation now better explains the "why" behind each middleware:
    *   `HttpLoggingMiddleware`: Emphasizes its high-performance design using `RecyclableMemoryStream` for safe use in production.
    *   `DefaultHeadersMiddleware`: Highlights its role in enforcing distributed tracing best practices by requiring `X-Request-Id`.
*   **Improved Usage Examples**: The code examples are now more explicit, showing exactly how to register the services and add the middlewares to the application pipeline.
*   **Example Error Response**: Added an example JSON response to the `README.md` to show what happens when the `DefaultHeadersMiddleware` rejects a request, making the behavior clear.

### 📝 Notes

The goal of this update is to enhance the developer experience by providing clear, actionable guidance. By improving the documentation, we aim to help users quickly and correctly configure essential cross-cutting concerns for their APIs, leading to better diagnostics and more consistent behavior across services.

