---
title: Overview
sidebar_position: 1
---

# Architecture Overview

## Overview

The solution divides into concentric layers with a single core principle: **dependencies point inward**.
Outer layers depend on inner layers — inner layers never depend on outer layers.
Business logic sits at the center, independent of frameworks, databases, and transport concerns.

```
        ┌─────────────────────────────────────┐
        │  Presentation (Api, Api.Contracts)  │
        │  ┌───────────────────────────────┐  │
        │  │  Infrastructure               │  │
        │  │  (Persistence, Integration)   │  │
        │  │  ┌─────────────────────────┐  │  │
        │  │  │  Application            │  │  │
        │  │  │  ┌───────────────────┐  │  │  │
        │  │  │  │      Domain       │  │  │  │
        │  │  │  └───────────────────┘  │  │  │
        │  │  └─────────────────────────┘  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘
```

## The Layers

### Domain

Resides in `src/Core/RaTemplate.Domain` as the innermost tier.
Holds organization-wide logic independent of technology stacks.

- **Entities**: Identity-focused objects representing core business concepts
- **Constants**: Shared value sets such as `EntitiesConstraints` and `ErrorCodes`
- **Domain logic**: Rules that must hold regardless of database, transport, or UI

*Maintains zero external project dependencies.*

### Application

Housed in `src/Core/RaTemplate.Application`.
Directs system workflows and depends only on Domain.

- **Commands & Queries**: State-changing actions and data-retrieval requests, handled via the Mediator pattern (`services.AddMediator()`)
- **Handlers**: Request processors generated per feature
- **Abstractions**: Contracts for external systems — e.g. the per-provider `IRaTemplateSqlServerDbContext`, `IRaTemplateOracleDbContext`, `IRaTemplatePostgresDbContext`, and `IRaTemplateSqliteDbContext` interfaces under `Abstractions/Data/`
- **Validators**: Integrity enforcement rules built on FluentValidation, registered automatically from the assembly

### Infrastructure

Positioned in `src/Infrastructure/`, split into three projects that fulfill Application contracts:

- **`RaTemplate.Infrastructure`**: Central wiring for infrastructure components and dependency injection (`AddInfrastructureServices`)
- **`RaTemplate.Persistence`**: EF Core data access — one `DbContext` per selected provider (SQL Server, Oracle, PostgreSQL, SQLite), plus `Schemas` and `RaTemplateDbInitializer` for development database setup. Excluded when the database is `None`
- **`RaTemplate.Integration`**: HTTP client infrastructure for consuming external APIs, including request/response logging. Excluded when `UseIntegrations` is disabled

### Presentation

Located in `src/Presentation/`. Serves as the system gateway, referencing Application and Infrastructure for composition-root setup only.

- **`RaTemplate.Api`**: Minimal API entry point. `Program.cs` wires logging, JSON options, health checks, OpenAPI (Scalar or Swagger), exception handling, JWT authorization (optional), and middleware via the `ServiceConfiguration/` extension classes (`AuthorizationExtensions`, `HealthCheckExtensions`, `MiddlewareExtensions`, `OpenApiExtensions`)
- **`RaTemplate.Api.Contracts`**: Shared request/response contracts with no project dependencies, consumable by clients without referencing the API

## Enforcing the Architecture

The `tests/RaTemplate.ArchitectureTests/` project uses NetArchTest to enforce the rules mechanically, not just by convention:

- `DependencyTests` — layer dependency direction (nothing points outward)
- `ApplicationTests`, `HandlersTests` — feature and handler conventions
- `EntitiesTests` — entity naming and structure rules
- `PersistenceTests`, `ApiTests` — data access and endpoint conventions

A freshly scaffolded solution passes all of them; any violation you introduce fails the build's test run.

## Why This Structure?

Each tier maintains a single purpose and sharp boundaries:

- **Testable**: Core tiers lack external dependencies, allowing isolated logic verification without live databases or servers
- **Replaceable**: Tooling stays behind contracts (the `Abstractions/Data` interfaces), enabling component swaps — even entire database providers — without altering central code
- **Discoverable**: Capabilities stack vertically per tier, streamlining feature location
- **Stable under change**: Central tiers resist outer fluctuations, shielding core logic from UI or database updates
- **Configurable**: Optional concerns (persistence, integrations, authorization) are separate projects or flags, so the generated solution stays free of unused code
