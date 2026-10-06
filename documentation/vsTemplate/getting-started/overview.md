---
title: Overview
sidebar_position: 1
---

# Overview

The **RA.CleanArchitecture.Template** is a .NET solution template for ASP.NET Core that implements Clean Architecture principles. It gives you a **production-ready starting point** so you can focus on building features, not boilerplate.

## What is Clean Architecture?

Clean Architecture separates code into distinct layers with strict dependency rules, keeping business logic independent of frameworks, databases, and UI concerns. Business logic sits at the center of the application, and outer layers depend inward — never the other way around.

## What the template generates

Running `dotnet new RA.Template -n YourProjectName` scaffolds a complete multi-project solution:

| Project | Type | Description |
| :--- | :--- | :--- |
| `Domain` | Class Library | Encapsulates core business entities, constants, and rules — no dependencies on other layers |
| `Application` | Class Library | Defines use cases (CQRS handlers, validators, DTOs) and abstractions for infrastructure concerns |
| `Infrastructure` | Class Library | Central wiring for infrastructure components and dependency injection |
| `Integration` _(optional)_ | Class Library | Pre-configured HTTP client infrastructure with request/response logging; included when `UseIntegrations` is enabled |
| `Persistence` _(optional)_ | Class Library | Entity Framework Core data access — SQL Server, Oracle, PostgreSQL, or SQLite (one `DbContext` per selected provider); excluded when the database is `None` |
| `Api` | ASP.NET Core Web API | Exposes the API, handles routing, and (optionally) JWT authorization |
| `Api.Contracts` | Class Library | Shared request/response contracts with no project dependencies |
| `ArchitectureTests` | Test Project | NetArchTest-based tests enforcing layer boundaries and naming conventions |

## Key features

- **.NET 10** target framework
- **CQRS, Dependency Injection, and Mediator** patterns built in
- **JWT authorization** — include or exclude with a single parameter
- **OpenAPI documentation** via Scalar, served at `/openapi-ui`
- **Flexible persistence** — choose one, several, or no database providers
- **Ready-to-use setup** — preconfigured logging, validation, and exception handling
- **Architecture tests** — layer boundaries enforced from day one

## Who it's for

The template is designed for .NET developers who:

- Are starting a new enterprise application and want a solid foundation
- Want to apply Clean Architecture without spending days wiring up infrastructure
- Value testability, maintainability, and separation of concerns
- Are building REST APIs, microservices, or enterprise apps that need to stay extensible