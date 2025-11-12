---
title: RaTemplate.Infrastructure
sidebar_position: 1
---

The `RaTemplate.Infrastructure` project is where the application interacts with the outside world.
In a **Clean Architecture** design, this layer contains the concrete implementations of the interfaces defined in the `Application` layer.
It handles all external concerns, such as databases, file systems, email providers, and external APIs.

This layer is considered the "details" of the architecture.
It is volatile and subject to change as technologies and external systems evolve.
The core business logic in the `Domain` and `Application` layers remains stable and unaware of the specific implementations in this project.

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Infrastructure` layer is to provide the concrete "how" for the abstract "what" defined by the `Application` layer's interfaces.

This project is responsible for:
*   **Implementing Application Interfaces**:
It provides concrete classes that implement the contracts (interfaces) from the `Application` layer, such as `IRepository`, `IDateTimeProvider`, or `IEmailSender`.

*   **Data Persistence**:
It contains the logic for accessing and storing data in a database.
This responsibility is often delegated to a dedicated `RaTemplate.Persistence` project, which is then referenced and configured here.

*   **External Service Integration**:
It includes the logic for communicating with other services or APIs.
This is typically handled in a separate `RaTemplate.Integration` project.

*   **Implementing Cross-Cutting Concerns**:
It provides implementations for services like logging, caching, and configuration management.

*   **Dependency Injection Registration**:
A key role of this project is to register all its concrete implementations with the dependency injection container, mapping them to the interfaces defined in the `Application` layer. 
his is typically done in a `*ServiceRegistration.cs` file.

## 🏛️ Architectural Principles

*   **Dependency Rule**:
This project depends on the `Application` layer to implement its interfaces.
The `Domain` and `Application` layers have **no dependency** on the `Infrastructure` layer.
This adheres to the Dependency Inversion Principle, where high-level modules do not depend on low-level modules; both depend on abstractions.

*   **Volatility**:
This layer is the most volatile part of the system.
Implementations can be swapped out with minimal impact on the core application.
For example, you could switch from Entity Framework to Dapper, or from SQL Server to Oracle, by changing only the code within this layer.

*   **Framework Dependencies**:
This is the appropriate place for dependencies on external libraries and frameworks like Entity Framework Core, Dapper, cloud SDKs, and other third-party packages.

## 📦 Contents

The `RaTemplate.Infrastructure` project acts as an aggregator and bootstrapper for other infrastructure-related projects.

*   **`InfrastructureServiceRegistration.cs`**: The entry point for registering all services from this layer and its sub-projects (`Persistence`, `Integration`) into the main application's dependency injection container.
*   **Services/**: May contain implementations for common services like a `DateTimeProvider` or `EmailSender`.

It typically references other specialized infrastructure projects:
*   **`RaTemplate.Persistence/`**: Contains all data access logic, including `DbContext` (for EF Core), repository implementations, and database migrations.
*   **`RaTemplate.Integration/`**: Contains clients and services for communicating with external APIs.
