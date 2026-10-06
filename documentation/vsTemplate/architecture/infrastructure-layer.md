---
title: Infrastructure Layer
sidebar_position: 4
---

# Infrastructure Layer

The Infrastructure layer implements the abstractions declared in Application and owns every technical concern the business logic shouldn't know about — database access, outgoing HTTP calls, health checks, and audit plumbing.
In this template it is **split across three projects** under `src/Infrastructure/`, and two of them are optional based on the parameters you chose at creation time.

| Project | Always present? | Responsibility |
| :--- | :--- | :--- |
| `RaTemplate.Infrastructure` | Yes | Composition aggregator — wires the other infrastructure projects into DI |
| `RaTemplate.Persistence` | Only when a database is selected | EF Core data access (one `DbContext` per provider) |
| `RaTemplate.Integration` | Only when `UseIntegrations` is `true` | HTTP client infrastructure with request/response logging |

All three reference the Application project (and Domain transitively). None of them is referenced by Application or Domain — outer layers depend inward, and the `DependencyTests` enforce it.

## RaTemplate.Infrastructure (the aggregator)

This small project is the single entry point the composition root calls. It holds no logic of its own; it forwards to Persistence and Integration depending on what was generated:

```csharp
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
{
    //#if (UseAnyDatabase)
    services.AddPersistence(configuration);
    //#endif
    //#if (UseIntegrations)
    services.AddIntegrationServices(configuration);
    //#endif

    return services;
}
```

Its project references to `RaTemplate.Persistence` and `RaTemplate.Integration` are themselves conditional, so a minimal API (`--database None --UseIntegrations false`) produces an aggregator that references neither.

## RaTemplate.Persistence

Implements the per-provider `IRaTemplate{Provider}DbContext` interfaces from `Application/Abstractions/Data/`. It references the `RA.Utilities.Data.EntityFramework` package plus, per selected provider, the matching EF Core provider and health-check packages (SQL Server, Oracle, PostgreSQL, SQLite).

### One DbContext per provider

For each database you selected, the template generates a **sealed** context that implements the corresponding Application interface. For example, SQL Server:

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

The Oracle, PostgreSQL, and SQLite contexts (`RaTemplateOracleDbContext`, `RaTemplatePostgresDbContext`, `RaTemplateSqliteDbContext`) follow the identical shape. Each keeps the context lightweight: a default schema (`Schemas.Default`, currently `"public"`) and reflection-based application of all `IEntityTypeConfiguration<>` in the assembly. Add your `DbSet<T>` properties here.

### Entity configuration (conventions enforced)

Mapping is expressed with EF Core `IEntityTypeConfiguration<T>` classes, auto-discovered by `ApplyConfigurationsFromAssembly`. `PersistenceTests` enforces the conventions:

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
        builder.OwnsOne(c => c.Email);
    }
}
```

### Auditing interceptor

Persistence registers `BaseEntitySaveChangesInterceptor` (from `RA.Utilities.Data.EntityFramework`) as a scoped service and attaches it to every context. It runs on each `SaveChanges`/`SaveChangesAsync` and stamps audit fields automatically:

- Entities in the **Added** state that expose `CreatedAt` get `DateTime.UtcNow`.
- Entities in the **Modified** state that expose `LastModifiedAt` get `DateTime.UtcNow`.

This is what makes the `BaseEntity<TKey>` / `WriteEntity<TKey>` audit columns from the [Domain layer](domain-layer.md) work with no per-handler code.

:::note
There is deliberately **no** EF-driven domain-event dispatcher. Cross-cutting reactions are modelled as mediator notifications published explicitly from handlers (see the [Application layer](application-layer.md)).
:::

### Options, logging, and connection strings

`ConfigureOptions<TContext>` attaches the interceptor to each context and, **in Development only**, enables EF command logging (`LogTo` at Information) and `EnableSensitiveDataLogging()`.

Each provider reads its own connection string by convention — `RaTemplate{Provider}ConnectionString` (e.g. `RaTemplateSqlServerConnectionString`). These keys are renamed along with your project when the solution is generated. A missing or blank connection string throws immediately at registration.

### Health checks

`AddPersistence` also registers a database health check per selected provider (`AddSqlServer`, `AddOracle`, `AddNpgSql`, `AddSqlite`), surfaced through the API's `/health` endpoint.

### Database initialization

`RaTemplateDbInitializer.InitializeDatabaseAsync` is invoked from `Program.cs` **only when the environment is Development**. It resolves the SQL Server context, runs `EnsureDeleted` then `EnsureCreated`, and logs the generated create script. A private `SeedAsync` stub is included for you to extend with default data (it is not wired in by default). Swap this delete-and-recreate strategy for migrations before going to production.

## RaTemplate.Integration

Provides the infrastructure for calling external HTTP services, built on the `RA.Utilities.Integrations` package. Its registration adds a request/response logging delegating handler:

```csharp
public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
{
    services.AddScopedHttpMessageHandler<RequestResponseLoggingHandler>();
    return services;
}
```

`RequestResponseLoggingHandler` is a `DelegatingHandler` that logs outgoing requests and responses. Register your typed `HttpClient`s against an abstraction declared in Application, attach this handler, and keep vendor SDKs out of the core:

```csharp
// Interface declared in Application, implemented here
services.AddHttpClient<ICustomerClient, CustomerClient>()
        .AddHttpMessageHandler<RequestResponseLoggingHandler>();
```

This project is omitted entirely when `UseIntegrations` is `false`.

## DI registration flow

The composition root calls one method, which cascades through the layer:

```text
Program.cs
  └─ AddInfrastructureServices(configuration)          (RaTemplate.Infrastructure)
       ├─ AddPersistence(configuration)                (RaTemplate.Persistence, if a DB is selected)
       │    ├─ AddDatabase      → interceptor, per-provider DbContext + interface mapping
       │    └─ AddHealthChecks  → per-provider DB health checks
       └─ AddIntegrationServices(configuration)        (RaTemplate.Integration, if enabled)
            └─ RequestResponseLoggingHandler
```

## Conditional generation

The Infrastructure layer is the most parameter-sensitive part of the solution:

- **`--database None`** → the entire `RaTemplate.Persistence` project, the `Abstractions/Data/` interfaces, and `PersistenceTests` are excluded; the aggregator drops its Persistence reference and `AddPersistence` call.
- **A specific provider not selected** → that provider's `DbContext`, its Application interface, its EF/health-check packages, its connection-string wiring, and its health check are all removed.
- **`--UseIntegrations false`** → the whole `RaTemplate.Integration` project is excluded.

The generated solution therefore contains only the infrastructure you asked for — no dead code.

## Dependencies

Infrastructure depends on Application (and Domain through it) and on framework/vendor packages. Nothing in Application or Domain references Infrastructure, so storage mechanisms and integration adapters can be replaced — or removed — without touching core business rules.
