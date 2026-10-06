---
title: Persistence Layer
sidebar_position: 5
---

# Persistence Layer

The Persistence layer lives in `src/Infrastructure/RaTemplate.Persistence` and implements the data-access side of the Application layer's `IRaTemplate{Provider}DbContext` abstractions using Entity Framework Core.
It is one of the three Infrastructure projects (see the [Infrastructure Layer](infrastructure-layer.md)) and is generated **only when you select at least one database provider** — with `--database None` the whole project, its abstractions, and its tests are removed.

:::info[Template state]
the scaffold ships one sealed `DbContext` per selected provider, the DI wiring, the audit interceptor, health checks, and a development-time initializer. It contains **no entities, entity configurations, or seed data yet** — you add those, and the architecture tests enforce the conventions below.
:::

## Project Dependencies

| Reference | Kind | Purpose |
| :--- | :--- | :--- |
| `RaTemplate.Application` | Project | The `IRaTemplate{Provider}DbContext` abstractions this layer implements |
| `RA.Utilities.Data.EntityFramework` | Package | `BaseEntitySaveChangesInterceptor` (audit timestamps) and optional generic repository bases |
| `Microsoft.Extensions.Configuration` | Package | Reading connection strings |
| `Microsoft.Extensions.Hosting.Abstractions` | Package | `IHostEnvironment` for environment-aware options |
| `Microsoft.EntityFrameworkCore.SqlServer` / `Oracle.EntityFrameworkCore` / `Npgsql.EntityFrameworkCore.PostgreSQL` / `Microsoft.EntityFrameworkCore.Sqlite` | Package | The EF Core provider(s) you selected |
| `AspNetCore.HealthChecks.{SqlServer,Oracle,NpgSql,Sqlite}` | Package | The matching database health check(s) |

Only the packages for the providers you selected are referenced; the rest are stripped by the template's conditional generation.

## One DbContext Per Provider

The defining feature of this layer is **multi-provider support**: each selected database gets its own sealed context, interface, connection string, and health check. This is not a single `DbContext` with a swappable provider — it is one concrete context per provider, so an application can target several databases at once.

| Provider | Concrete context | Application abstraction | EF call | Health check | Connection-string key |
| :--- | :--- | :--- | :--- | :--- | :--- |
| SQL Server | `RaTemplateSqlServerDbContext` | `IRaTemplateSqlServerDbContext` | `UseSqlServer` | `AddSqlServer` | `RaTemplateSqlServerConnectionString` |
| Oracle | `RaTemplateOracleDbContext` | `IRaTemplateOracleDbContext` | `UseOracle` | `AddOracle` | `RaTemplateOracleConnectionString` |
| PostgreSQL | `RaTemplatePostgresDbContext` | `IRaTemplatePostgresDbContext` | `UseNpgsql` | `AddNpgSql` | `RaTemplatePostgresConnectionString` |
| SQLite | `RaTemplateSqliteDbContext` | `IRaTemplateSqliteDbContext` | `UseSqlite` | `AddSqlite` | `RaTemplateSqliteConnectionString` |

Each concrete context is **sealed**, uses a C# primary constructor for its `DbContextOptions<T>`, and implements the matching Application interface — so handlers depend on the abstraction, never on the concrete context.

## Context Anatomy

The SQL Server context sets a default schema; the others do not:

```csharp
public sealed class RaTemplateSqlServerDbContext(DbContextOptions<RaTemplateSqlServerDbContext> options)
    : DbContext(options), IRaTemplateSqlServerDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RaTemplateSqlServerDbContext).Assembly);
    }
}
```

```csharp
public sealed class RaTemplateSqliteDbContext(DbContextOptions<RaTemplateSqliteDbContext> options)
    : DbContext(options), IRaTemplateSqliteDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RaTemplateSqliteDbContext).Assembly);
    }
}
```

:::note[Provider difference:]
only `RaTemplateSqlServerDbContext` calls `HasDefaultSchema(Schemas.Default)`. The Oracle, PostgreSQL, and SQLite contexts apply configurations but leave the schema to the provider/connection default (Oracle uses the connected user's schema; SQLite has no schema concept).
:::

Both `OnModelCreating` overrides call `ApplyConfigurationsFromAssembly`, so every `IEntityTypeConfiguration<>` in the assembly is discovered and applied automatically — no manual wiring per entity.

The `Schemas` holder is internal:

```csharp
internal static class Schemas
{
    public const string Default = "public";
}
```

Add your `DbSet<T>` properties to the relevant context(s) as you introduce entities.

## Entity Configuration (Conventions Enforced)

Mapping is expressed with EF Core `IEntityTypeConfiguration<TEntity>` classes, auto-applied by `ApplyConfigurationsFromAssembly`. `PersistenceTests` in `tests/RaTemplate.ArchitectureTests/` enforces the conventions:

| Rule | Requirement |
| :--- | :--- |
| Type | Implements `IEntityTypeConfiguration<TEntity>` |
| Visibility | **Not public** (internal) |
| Sealed | Yes |
| Suffix | Ends with `Config` |
| Naming | `{Entity}Config` — e.g. `IEntityTypeConfiguration<User>` must be named `UserConfig` |
| Namespace | Lives in `RaTemplate.Persistence.Configuration` |

```csharp
namespace RaTemplate.Persistence.Configuration;

internal sealed class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(EntitiesConstraints.CustomerNameMaxLength).IsRequired();
        builder.OwnsOne(c => c.Email);   // value objects from the Domain layer
    }
}
```

A configuration that is public, unsealed, misnamed, or outside the `Configuration` namespace fails the architecture test run.

## Auditing Interceptor

Persistence registers `BaseEntitySaveChangesInterceptor` (from `RA.Utilities.Data.EntityFramework`) as a **scoped** service and attaches it to every context. It runs on each `SaveChanges`/`SaveChangesAsync` and stamps audit fields automatically, based on the entity's `EntityState`:

- **Added** entities that expose a `CreatedAt` property → set to `DateTime.UtcNow`.
- **Modified** entities that expose a `LastModifiedAt` property → set to `DateTime.UtcNow`.

This is what makes the `BaseEntity<TKey>` / `WriteEntity<TKey>` audit columns introduced in the [Domain Layer](domain-layer.md) work with no per-handler code. The interceptor is property-based (it checks whether the mapped property exists), so contexts whose entities don't derive from those bases are unaffected.

:::warning
There is deliberately **no** EF-driven domain-event dispatcher.
Cross-cutting reactions are modelled as mediator notifications published explicitly from handlers (see the [Application Layer](application-layer.md)), not as events flushed by the context.
:::

## Context Options & Logging

A shared `ConfigureOptions<TContext>` runs for every provider when its context is registered. It:

1. Attaches the `BaseEntitySaveChangesInterceptor`.
2. **In Development only**, enables EF Core command logging via `LogTo` (at `Information`, under the `Database.Command` category) and turns on `EnableSensitiveDataLogging()`.

Sensitive-data logging is gated on the Development environment, so parameter values never leak into production logs.

## Connection Strings

`GetConnectionString(configuration, name)` reads each key and calls `ArgumentException.ThrowIfNullOrWhiteSpace`, so a missing or blank connection string fails fast at registration rather than at first query. The keys are empty in the generated `appsettings.json` and are renamed along with your project:

```jsonc
{
  "ConnectionStrings": {
    "RaTemplateSqlServerConnectionString": "",
    "RaTemplatePostgresConnectionString": ""
  }
}
```

Fill in one entry per selected provider before running.

## Health Checks

`AddPersistence` registers a database health check per selected provider (`AddSqlServer`, `AddOracle`, `AddNpgSql`, `AddSqlite`), each pointed at its connection string. These compose with the API's `self` liveness check and surface at the `/health` endpoint (see the [Presentation Layer](presentation-layer.md)).

## Database Initialization

`RaTemplateDbInitializer.InitializeDatabaseAsync` is invoked from `Program.cs` **only when the environment is Development**. It creates a scope, resolves the SQL Server context, runs `EnsureDeletedAsync` then `EnsureCreatedAsync`, and logs the generated create script. A private `SeedAsync` stub is included for you to extend with default data (it is not called by default).

:::danger
The delete-and-recreate strategy is for local development convenience. Replace it with EF Core **migrations** before deploying to any shared or production environment.
:::

## Optional Repository Base

The `RA.Utilities.Data.EntityFramework` package also ships generic repository bases (`RepositoryBase`, `ReadRepositoryBase`, `WriteRepositoryBase`, and their interfaces). The template does **not** register them by default — handlers depend on the per-provider context abstraction directly. You can introduce repositories over the contexts if your data access warrants it.

## DI Registration

Persistence is wired through `AddPersistence`, called by the Infrastructure aggregator (`AddInfrastructureServices`) only when a database is selected:

```csharp
public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
{
    services.AddDatabase(configuration)
            .AddHealthChecks(configuration);

    return services;
}
```

- `AddDatabase` registers the scoped interceptor and, per selected provider, calls `AddDbContext<RaTemplate{Provider}DbContext>(...)` (using `ConfigureOptions<TContext>`) and maps the interface to the concrete context: `AddScoped<IRaTemplate{Provider}DbContext>(sp => sp.GetRequiredService<RaTemplate{Provider}DbContext>())`.
- `AddHealthChecks` (private) adds the per-provider database checks.

Each provider's registration block is wrapped in a template conditional (`UseEfSqlServer`, `UseEfOracle`, `UseEfPostgres`, `UseEfSqlite`), so only the selected providers are compiled into the generated project.

## Conditional Generation

- **`--database None`** → the entire `RaTemplate.Persistence` project, the `Abstractions/Data/` interfaces, and `PersistenceTests` are excluded; the Infrastructure aggregator drops its Persistence reference and `AddPersistence` call.
- **An individual provider not selected** → that provider's context, its Application interface, its EF and health-check packages, its `AddDbContext`/interface mapping, its health check, and its connection-string key are all removed.

## Dependencies

Persistence depends on Application (for the context abstractions) and on EF Core / health-check / `RA.Utilities` packages. Nothing in Application or Domain references Persistence — handlers consume the `IRaTemplate{Provider}DbContext` interfaces, so the data store can be changed or removed without touching core business rules. The `DependencyTests` enforce this direction.
