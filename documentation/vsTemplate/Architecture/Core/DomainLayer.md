---
title: RaTemplate.Domain
sidebar_position: 2
---

The `RaTemplate.Domain` project is the absolute core of the application, representing the very heart of the business logic.
In a Clean Architecture design, this layer encapsulates the most fundamental and high-level rules of the system.
It is designed to be completely independent of any other layer, ensuring that the business logic remains pure and uncoupled from infrastructure concerns like databases, user interfaces, or external services.

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Domain` layer is to model the business domain.
It contains all the enterprise-wide logic and data structures that are essential to the application's function, regardless of how it's delivered or where its data is stored.

This project is responsible for:
*   **Defining Business Entities**: These are the core objects of your domain with a distinct identity that persists through different states of the application (e.g., `Product`, `Order`, `Customer`).
*   **Defining Value Objects**: These are immutable objects that represent descriptive aspects of the domain without a conceptual identity (e.g., `Address`, `Money`).
*   **Encapsulating Domain Logic**: It contains the business rules, validations, and logic that govern the state of the entities. This logic should be rich and expressive.
*   **Defining Domain Events**: These are events that occur within the domain that other parts of the application might be interested in (e.g., `OrderCreated`, `ProductPriceChanged`).
*   **Defining Enums**: Contains enumerations that are specific to the business domain.

##  🏛️ Architectural Principles

*   **Independence**: This project has **zero dependencies** on any other project within the solution.
It does not know about the `Application`, `Infrastructure`, or `Api` layers. This is the most critical rule of the Domain layer.

*   **Persistence Ignorance**: The domain entities are completely unaware of how they are persisted.
They are simple C# objects (POCOs) that do not contain any database-specific attributes or logic.

*   **High-Level Policies**: This layer contains the highest-level policies and business rules of the application.

## 📦 Contents

A typical `RaTemplate.Domain` project includes the following folders:

*   `Entities/`: Contains the core domain entities.
*   `Enums/`: Holds domain-specific enumerations.
*   `Events/`: For defining domain events.
*   `Exceptions/`: Custom exceptions that are part of the domain's language.
*   `ValueObjects/`: Contains the domain's value objects.
