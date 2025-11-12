---
title: RaTemplate.Persistence
sidebar_position: 2
---

The `RaTemplate.Persistence` project is a specialized infrastructure layer responsible for all data access and persistence concerns.
In a **Clean Architecture** design, this project provides the concrete implementation for the data-related interfaces defined in the `Application` layer.
It is the component that knows how to talk to the database.

This project is a "detail" of the architecture, meaning the core application (`Domain` and `Application` layers) is completely unaware of its existence or the specific technologies it uses (e.g., Entity Framework Core, SQL Server).

## 🎯 Purpose and Key Responsibilities

The primary purpose of the `Persistence` layer is to handle the storage and retrieval of domain entities from a database.

This project is responsible for:
*   **Implementing Repository Interfaces**: It provides concrete repository classes (e.g., `EfProductRepository`) that implement the `IRepository` interfaces defined in the `Application` layer.
*   **Defining the DbContext**: It contains the `DbContext` class (e.g., `RaTemplateDbContext`), which represents a session with the database and allows for querying and saving data. This is the central class for Entity Framework Core.
*   **Configuring Entities**: It defines the database schema, constraints, and relationships for the domain entities using EF Core's Fluent API or data annotations.
*   **Handling Database Migrations**: It is the home for all database migration files, which track incremental changes to the database schema over time.
*   **Implementing the Unit of Work Pattern**: The `DbContext` naturally implements the Unit of Work pattern, grouping multiple database operations into a single transaction.

## 🏛️ Architectural Principles

*   **Dependency Rule**: This project depends on the `Application` layer to access the repository interfaces it needs to implement. It also depends on the `Domain` layer to access the entities it will be persisting. The core layers (`Domain`, `Application`) have **no dependency** on this project.
*   **Implementation Detail**: The choice of ORM (Entity Framework Core, Dapper) and database (SQL Server, Oracle) are implementation details confined to this project. This allows the data access technology to be changed with minimal impact on the rest of the application.
*   **Framework-Specific**: This is the correct place to have dependencies on data-access-specific NuGet packages like `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, or `Dapper`.

## 📦 Contents

A typical `RaTemplate.Persistence` project includes the following folders:

*   **`Database/`**: Contains the `RaTemplateDbContext` class.
*   **`Configurations/`**: Holds the EF Core entity type configuration classes (using `IEntityTypeConfiguration<TEntity>`).
*   **`Repositories/`**: Contains the concrete implementations of the repository interfaces from the `Application` layer.
*   **`Migrations/`**: Stores the auto-generated database migration files.
*   **`PersistenceDependencyInjection.cs`**: A static class that contains the extension method (`AddPersistence`) to register all the services from this project (like the `DbContext` and repositories) into the application's dependency injection container.

This project is referenced by the main `RaTemplate.Infrastructure` project, which then calls its service registration method to wire everything up at application startup.
