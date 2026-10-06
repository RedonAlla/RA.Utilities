---
title: Application Layer
sidebar_position: 3
---

# Application Layer

The Application layer lives in `src/Core/RaTemplate.Application`.
It orchestrates use cases: it depends on **Domain only** and defines the abstractions that Infrastructure must implement.
It has no knowledge of HTTP, databases, or the file system — everything external is reached through an interface.

:::info[Template state:]
the scaffold ships the wiring (`DependencyInjection.cs`, `AssemblyReference.cs`) and the per-provider data abstractions under `Abstractions/Data/`. It contains no sample features yet — you add your own commands, queries, handlers, and validators, and the architecture tests enforce the conventions below.
:::

## Project Dependencies

| Reference | Kind | Purpose |
| :--- | :--- | :--- |
| `RaTemplate.Domain` | Project | Core entities, value objects, and constants |
| `RA.Utilities.Feature` | Package | The source-generated mediator (`IMediator`), handler/behavior abstractions, and pipeline behaviors |
| `FluentValidation.DependencyInjectionExtensions` | Package | Assembly-scanning registration of validators |

## The Mediator: `RA.Utilities.Feature`

This template does **not** use MediatR.
It uses `RA.Utilities.Feature`, a reflection-free, source-generated mediator:

- Handlers and closed pipeline behaviors are discovered at compile time by an in-package source generator and registered when you call `AddMediator()` — plain handlers need no explicit registration code.
- `IMediator.Send(...)` returns the response value directly; failures surface as **typed exceptions** (`NotFoundException`, `ConflictException`, `BadRequestException`, …) rather than a `Result` to unwrap. The API layer's global exception handler maps them to HTTP responses.
- You can inject `IMediator` or the concrete generated `Mediator` for the fastest monomorphized dispatch.

The core abstractions you implement live in `RA.Utilities.Feature.Abstractions`:

| Abstraction | Role |
| :--- | :--- |
| `IRequest` / `IRequest<TResponse>` | A feature input (command or query), with or without a response |
| `IRequestHandler<TRequest, TResponse>` / `IRequestHandler<TRequest>` | Handles a request; the method is `HandleAsync` |
| `RequestHandler<TRequest, TResponse>` / `RequestHandler<TRequest>` | Optional abstract base that removes interface plumbing |
| `INotification` / `INotificationHandler<TNotification>` | Publish/subscribe domain events (fan-out to zero or more handlers) |
| `IPipelineBehavior<TRequest, TResponse>` / `IPipelineBehavior<TRequest>` | Cross-cutting decorators around a request |

## Vertical Slices

Each capability lives in its own folder — the request, handler, validator, and any DTOs sit together, so adding a feature means adding a directory rather than inflating a central service class. A recommended layout:

```text
RaTemplate.Application/
    Features/
        Customers/
            Commands/
                CreateCustomer/
                    CreateCustomerInput.cs
                    CreateCustomerHandler.cs
                    CreateCustomerValidator.cs
                    CustomerOutput.cs
            Queries/
                GetCustomer/
                    GetCustomerInput.cs
                    GetCustomerHandler.cs
                    CustomerOutput.cs
    Abstractions/
        Data/
```

## Use Cases That Mutate State

A command is a sealed record implementing `IRequest` (no response) or `IRequest<TResponse>`, with a matching handler whose method is `HandleAsync`. Persistence is reached through the injected data abstraction.

```csharp
namespace RaTemplate.Application.Features.Customers.Commands.CreateCustomer;

public sealed record CreateCustomerInput(string Name, string Email) : IRequest<Guid>;

public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerInput, Guid>
{
    private readonly IRaTemplateSqlServerDbContext _context;

    public CreateCustomerHandler(IRaTemplateSqlServerDbContext context) => _context = context;

    public async Task<Guid> HandleAsync(CreateCustomerInput request, CancellationToken cancellationToken)
    {
        var customer = new Customer { Name = request.Name, Email = new Email(request.Email) };
        // add via the DbSet exposed by the context abstraction
        await _context.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
```

For a handler that returns nothing, implement `IRequestHandler<TRequest>` and the parameterless `Task HandleAsync(...)`.

## Use Cases That Read Data

Queries follow the same shape and return DTOs. Their response types end in `Output` (see conventions below). Reads should use no-tracking projections where possible.

```csharp
public sealed record GetCustomerInput(Guid Id) : IRequest<CustomerOutput>;

public sealed class GetCustomerHandler : IRequestHandler<GetCustomerInput, CustomerOutput>
{
    private readonly IRaTemplateSqlServerDbContext _context;

    public GetCustomerHandler(IRaTemplateSqlServerDbContext context) => _context = context;

    public Task<CustomerOutput> HandleAsync(GetCustomerInput request, CancellationToken cancellationToken)
        => // project the entity to CustomerOutput
           throw new NotImplementedException();
}

public sealed record CustomerOutput(Guid Id, string Name, string Email);
```

## Interfaces & Abstractions

The Application layer declares the contracts that Infrastructure fulfils, so no inner code references a lower tier directly. The database contract is **per provider**, under `Abstractions/Data/` — the template generates one interface for each database you selected:

| Interface (when selected) | Implemented in |
| :--- | :--- |
| `IRaTemplateSqlServerDbContext` | `RaTemplate.Persistence` (SQL Server) |
| `IRaTemplateOracleDbContext` | `RaTemplate.Persistence` (Oracle) |
| `IRaTemplatePostgresDbContext` | `RaTemplate.Persistence` (PostgreSQL) |
| `IRaTemplateSqliteDbContext` | `RaTemplate.Persistence` (SQLite) |

Each exposes at least `Task<int> SaveChangesAsync(CancellationToken cancellationToken)`; add `DbSet<T>` properties for your entities as you introduce them. Handlers depend on the interface for the provider they target — never on the concrete EF Core `DbContext`.

Add other abstractions the same way (e.g. an `ICurrentUser`, `IDateTime`, or an integration client interface), defined here and implemented in Infrastructure or Integration.

## Domain Event Handlers

Cross-cutting reactions to state changes are modelled as notifications, keeping side effects out of both the Domain and the command handler. Publish with `IMediator.Publish(...)`; each `INotificationHandler<TNotification>` is invoked independently, so one failing handler does not block the others.

```csharp
public sealed record CustomerCreatedNotification(Guid CustomerId) : INotification;

public sealed class CustomerCreatedDecorator : INotificationHandler<CustomerCreatedNotification>
{
    public Task HandleAsync(CustomerCreatedNotification notification, CancellationToken cancellationToken)
        => Task.CompletedTask; // send email, update a read model, start a workflow, etc.
}
```

:::info
Notification handlers follow the same naming rule as pipeline behaviors — see conventions below.
:::

## Pipeline Behaviours (Decorators)

Behaviors wrap every request and run in order around the handler. `RA.Utilities.Feature` ships built-in behaviors, including a `ValidationBehavior` that runs your FluentValidation validators before the handler and short-circuits by throwing a `BadRequestException` carrying the structured errors — so handlers only ever see valid input. Logging, metrics, and retry behaviors are also available for notifications.

Registration follows the package's auto-registration rules:

- **Closed (non-generic) behaviors** are discovered and registered automatically by the source generator.
- **Generic behaviors** (e.g. `ValidationBehavior<TRequest, TResponse>`) are *not* auto-registered; wire them explicitly with `.AddDecoration<...>()`.

In this template, any custom `IPipelineBehavior<,>` must be named with the `Decorator` suffix (enforced — see below).

## Validation

Validators inherit FluentValidation's `AbstractValidator<TInput>` and are registered by assembly scanning in `AddApplicationServices()`. They must carry the `Validator` suffix.

```csharp
public sealed class CreateCustomerValidator : AbstractValidator<CreateCustomerInput>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(EntitiesConstraints.CustomerNameMaxLength);
        RuleFor(x => x.Email).NotEmpty();
    }
}
```

## Enforced Conventions

`ApplicationTests` and `HandlersTests` in `tests/RaTemplate.ArchitectureTests/` enforce the feature naming and sealing rules at test time:

| Component | Must end with | Must be |
| :--- | :--- | :--- |
| Feature input (`IRequest` / `IRequest<>`) | `Input` | `sealed` |
| Feature output (handler `TResponse`) | `Output` | — |
| Handler (`IRequestHandler<>` / `IRequestHandler<,>`) | `Handler` | `sealed` |
| Validator (`AbstractValidator<>`) | `Validator` | `sealed` |
| Decorator (`IPipelineBehavior<>` / `INotificationHandler<>`) | `Decorator` | `sealed` |

A feature that violates a suffix or sealing rule fails the architecture test run.

## DI Registration

Everything is wired through a single extension called from the composition root (`Program.cs`):

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    _ = services.AddMediator();

    return services;
}
```

`AddMediator()` applies the compile-time handler/behavior registrations produced by the source generator and registers `IMediator` (the generated `Mediator`). `AssemblyReference` is a marker type used to locate this assembly for scanning.

## Dependencies

The Application layer references **only** Domain (plus the `RA.Utilities.Feature` and FluentValidation packages).
It never references Infrastructure, Persistence, Integration, or Presentation — a rule the `DependencyTests` enforce.
Outer layers depend on this one, consuming its inputs, handlers, and abstractions.
