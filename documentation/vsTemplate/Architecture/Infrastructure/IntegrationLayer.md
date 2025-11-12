---
title: RaTemplate.Integration
sidebar_position: 3
---


The `RaTemplate.Integration` project is a specialized infrastructure layer responsible for all communication with external services and APIs.
In a **Clean Architecture** design, this project provides the concrete implementations for the integration-related interfaces defined in the `Application` layer. It is the component that knows how to talk to other systems over the network.

This project is considered a "detail" of the architecture. The core application (`Domain` and `Application` layers) is completely unaware of the specific external services being called or the protocols being used.

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Integration` layer is to abstract away the complexities of interacting with external APIs.

This project is responsible for:
*   **Implementing Integration Interfaces**: It provides concrete client classes that implement the contracts (e.g., `IExternalProductService`) defined in the `Application` layer.
*   **Containing HTTP Clients**: It uses `IHttpClientFactory` to create and manage `HttpClient` instances for making API calls.
*   **Handling External Data Models**: It often defines its own set of Data Transfer Objects (DTOs) to represent the data structures of the external APIs it communicates with. This prevents the external API's data model from leaking into the core application.
*   **Managing External Service Configuration**: It handles the configuration details for external services, such as base URLs, API keys, and timeouts.
*   **Implementing Cross-Cutting Concerns for HTTP**: It can add delegating handlers to the HTTP client pipeline for concerns like logging, retries, or adding custom headers. The `RequestResponseLoggingHandler` is a good example of this.

## 🏛️ Architectural Principles

*   **Dependency Rule**: This project depends on the `Application` layer to access the integration interfaces it needs to implement. The core layers (`Domain`, `Application`) have **no dependency** on this project. This follows the Dependency Inversion Principle.
*   **Implementation Detail**: The specific external service being called and the libraries used to do so (e.g., `HttpClient`, `RestSharp`) are implementation details confined to this project. This allows you to change or replace an external service provider with minimal impact on the core application.
*   **Framework-Specific**: This is the correct place for dependencies on HTTP-related or integration-specific NuGet packages like `Microsoft.Extensions.Http`, `Polly`, or SDKs for specific services (e.g., AWS, Azure).

## 📦 Contents

A typical `RaTemplate.Integration` project includes the following folders:

*   **`Clients/`** or **`Services/`**: Contains the concrete implementations of the client services that talk to external APIs.
*   **`DTOs/`**: Holds DTOs that match the request/response models of the external APIs.
*   **`Handlers/`**: Contains custom `DelegatingHandler` implementations for the HTTP client pipeline.
*   **`IntegrationServiceRegistration.cs`**: A static class with an extension method (`AddIntegrationServices`) to register all the services from this project (like typed HTTP clients and handlers) into the application's dependency injection container.

This project is referenced by the main `RaTemplate.Infrastructure` project, which calls its service registration method to wire everything up at application startup.
