---
title: RaTemplate.Application
sidebar_position: 3
---

The `RaTemplate.Application` project acts as the orchestrator for the domain logic.
In a Clean Architecture design, this layer contains the application-specific business rules and use cases.
It mediates between the presentation layer (`Api`) and the core `Domain` layer, ensuring that application logic is kept separate from both UI concerns and core business rules.

This layer defines the jobs the application can do, but it does not know how the external-facing layers (like the API or a database) work.

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Application` layer is to execute the application's use cases.
It coordinates the retrieval and modification of domain objects and directs the `Domain` layer to perform its work.

This project is responsible for:
*   **Implementing Use Cases**: Contains the logic for specific application operations, often implemented using patterns like CQRS (Command Query Responsibility Segregation).
    *   **Commands**: Handle requests that change the state of the system (e.g., `CreateProductCommand`).
    *   **Queries**: Handle requests that retrieve data without changing state (e.g., `GetProductByIdQuery`).
*   **Defining Data Transfer Objects (DTOs)**: These are simple data structures used to transfer data between layers, particularly between the `Api` and `Application` layers. This prevents domain entities from being directly exposed to the outside world.
*   **Defining Interfaces for Infrastructure Concerns**: It defines the contracts (interfaces) that the `Infrastructure` layer must implement. For example, `IRepository`, `IDateTimeProvider`, or `IEmailSender`. This follows the Dependency Inversion Principle.
*   **Handling Cross-Cutting Concerns**: Implements cross-cutting concerns like validation, logging, and caching that apply to application use cases.
*   **Defining Application-Specific Exceptions**: Custom exceptions that are relevant to the application's use cases (e.g., `NotFoundException`).

## 🏛️ Architectural Principles

*   **Dependency Rule**: This project depends on the `Domain` project. It can reference `Entities`, `ValueObjects`, and `Domain Events`. However, it must **not** have any dependencies on the `Infrastructure` or `Api` layers.
*   **Infrastructure Abstraction**: It defines interfaces for infrastructure services, but it has no knowledge of the concrete implementations (which reside in the `Infrastructure` project).

## 📦 Contents

A typical `RaTemplate.Application` project includes the following folders:

*   `CQRS/` or `Features/`: Organizes the application logic by feature, containing `Commands`, `Queries`, and their `Handlers`.
*   `DTOs/`: Contains Data Transfer Objects.
*   `Interfaces/`: Holds the interfaces for repositories and other infrastructure services.
*   `Validators/`: Contains validation logic for commands and DTOs (e.g., using FluentValidation).
*   `Mappings/`: Defines object-to-object mapping configurations (e.g., for AutoMapper).
