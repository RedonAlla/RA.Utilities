---
title: RaTemplate.Api
sidebar_position: 2
---

The `RaTemplate.Api` project is the presentation layer and the entry point of the application.
In a **Clean Architecture** design, this is the outermost layer, responsible for exposing the application's functionality to the outside world via a Web API.
It handles all HTTP-specific concerns and translates incoming requests into calls to the `Application` layer.

This project is the "delivery mechanism." It knows how to communicate over web protocols but contains no business logic itself.

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Api` layer is to handle web requests and act as the composition root for the entire application.

This project is responsible for:
*   **Exposing API Endpoints**: It defines the API endpoints that clients will interact with. This template uses a feature-based approach with Minimal APIs.
*   **Handling HTTP Requests/Responses**: It receives HTTP requests, deserializes request bodies into DTOs or CQRS commands/queries, and serializes the results from the `Application` layer back into HTTP responses.
*   **Orchestrating Application Logic**: It calls into the `Application` layer (typically using a mediator pattern like MediatR) to execute the relevant use case.
*   **Composition Root**: This is where all the application's services are "wired up." The `Program.cs` and `StartupExtensions.cs` files configure the dependency injection container, registering services from the `Application` and `Infrastructure` layers.
*   **Implementing Web-Specific Cross-Cutting Concerns**: It handles concerns that are specific to the web layer, such as:
    *   Global exception handling (translating exceptions to appropriate HTTP status codes).
    *   Authentication and Authorization middleware.
    *   Configuring and serving OpenAPI (Swagger/Scalar) documentation.
    *   CORS policies and rate limiting.

## 🏛️ Architectural Principles

*   **Dependency Rule**: This project depends on the `Application` and `Infrastructure` projects.
This is necessary because it acts as the composition root, where all dependencies are registered.
The flow of control, however, still follows the Dependency Rule: `Api` calls `Application`, which calls `Domain`.

*   **No Business Logic**: This layer should be "thin." It should not contain any business rules or domain logic.
Its job is to simply pass requests to the `Application` layer and return the result.

*   **UI-Specific**: All code in this project should be related to the API.
If you were to create a different type of UI (e.g., a gRPC service or a Blazor Web App), you would create a new presentation layer project, and it would replace this one, while the `Core` (`Domain` and `Application`) layers would remain unchanged.

## 📦 Contents

A typical `RaTemplate.Api` project includes the following:

*   **`Program.cs`**: The main entry point of the application where the web host is configured and started.
*   **`StartupExtensions.cs`**: Contains extension methods to keep `Program.cs` clean by organizing service registration and middleware configuration.
*   **`Endpoints/`**: Organizes API endpoints by feature, promoting a clean and scalable structure.
*   **`Extensions/`**: Contains extension methods for configuring specific services like Authorization or OpenAPI.
*   **`appsettings.json`**: Configuration files for the application.
