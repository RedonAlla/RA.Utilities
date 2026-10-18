# RA.Utilities.Dispatcher

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Dispatcher.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Dispatcher.svg)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Application/Dispatcher/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)


`RA.Utilities.Dispatcher` provides a foundational toolkit for implementing the **Vertical Slice Architecture** pattern using CQRS (Command Query Responsibility Segregation).
It includes a custom mediator, base handlers, pipeline behaviors for cross-cutting concerns, and a notification system to streamline feature development and promote clean, maintainable code.

Building applications with a traditional layered architecture can lead to wide, coupled classes and scattered logic.
The Vertical Slice pattern, combined with CQRS, addresses this by organizing code around features.
This package provides the essential building blocks to support that pattern.

> **Result-based dispatch redesign**: Handlers and `IDispatcher` now return `ValueTask<Result<T>>` / `ValueTask<Result>` using `RA.Utilities.Core.Results`. Expected failures are returned as `Result.Failure` instead of being thrown — `ValidationBehavior` returns a failed result containing a `BadRequestException`. Handlers are still **registered automatically** by a source generator shipped in this package — plain handlers need no explicit registration at all.

## Getting started

```bash
dotnet add package RA.Utilities.Dispatcher
```

---

## 🔗 Dependencies

-   [`RA.Utilities.Core.Results`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Results/)
-   [`RA.Utilities.Core.Exceptions`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Exceptions/)
-   [`RA.Utilities.Application.Validation`](https://redonalla.github.io/RA.Utilities/nuget-packages/application/RA.Utilities.Application.Validation/)
-   [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/)
-   [`Microsoft.Extensions.DependencyInjection.Abstractions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection)
-   [`Microsoft.Extensions.Logging.Abstractions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.abstractions)

---

## ✨ Features

### 1. Source-Generated Mediator

The package provides its own lightweight mediator — no external MediatR dependency. The **`IDispatcher` implementation is source-generated** into your assembly by the package's generator and registered by `AddMediator()`. It supports:

- **Request/Response** dispatch with a composable pipeline of behaviors
- **Notification** publishing to zero or more handlers, each wrapped in its own behavior pipeline
- Behavior ordering: behaviors execute in registration order, outermost first

`Send` returns `ValueTask<Result<TResponse>>` (`ValueTask<Result>` for void requests). Handlers, behaviors, and the generated dispatcher all speak `Result`; expected failures are returned as `Result.Failure(exception)` instead of being thrown. `Result` comes from `RA.Utilities.Core.Results`.

```csharp
Result<Pong> result = await mediator.Send(new Ping("hello"));
if (result.IsFailure)
{
    Exception error = result.Exception;
}
else
{
    Pong pong = result.Value;
}
```

Register the mediator once at startup:

```csharp
builder.Services.AddMediator();
```

### 2. Zero-Configuration Handler Registration

As of v11, plain handlers register themselves. A **source generator** shipped inside the `RA.Utilities.Dispatcher` NuGet package discovers every non-abstract class implementing `IRequestHandler<TRequest, TResponse>`, `IRequestHandler<TRequest>`, or `INotificationHandler<TNotification>` and emits a module initializer that queues their DI registrations. Calling `AddMediator()` applies the queued registrations:

- Request handlers are registered **scoped** (`IRequestHandler<TRequest, TResponse>` / `IRequestHandler<TRequest>`)
- Notification handlers are registered **transient** (`INotificationHandler<TNotification>`)

```csharp
// No AddDispatcher/AddNotification calls needed for plain handlers —
// AddMediator() registers every handler in the assembly.
builder.Services.AddMediator();
```

**Still registered explicitly** (the generator cannot discover or must not auto-register these):

- **Pipeline behaviors** — `AddDispatcher<TRequest, TResponse, THandler>().AddDecoration<TBehavior>()`
- **Validators** — `.AddValidator<TValidator>()` (which also wires the `ValidationBehavior`)
- **Notification behaviors** — `.AddNotification<TNotification>().AddDecoration<TBehavior>()`
- **Generic handlers** — the generator reports `DSAG002` and skips them
- Any handler whose registration needs a custom lifetime or setup

**Assembly-loading caveat**: auto-registration covers the assembly that calls `AddMediator()` plus any assembly already loaded before that call (module initializers run when an assembly loads). Referenced class libraries load lazily on the CLR, so a handler in a library whose types are first touched at request time may be registered too late. Keep handlers in the entry/composition assembly, touch the handler assembly at startup (e.g. `typeof(SomeHandler)`), or register those handlers explicitly.

### 3. Generated Mediator Details

The generated `Mediator` is the **only** `IDispatcher` implementation in the package:

- **Direct handler injection** — the generator knows each message's handler, registers it under its interface and as itself, and the generated `Send` resolves the handler by **concrete type** and calls it directly — no interface resolution, and the call devirtualizes.
- **No delegate chain without behaviors** — when a request has no pipeline behaviors, the handler is invoked directly. Benchmarks (MediatR handlers registered scoped, matching this package's lifetime) show **14-18% faster sends and 9-14% fewer allocations than MediatR** across the send shapes.
- **Closed behaviors auto-register** — non-generic `IPipelineBehavior<,>` / `IPipelineBehavior<>` / `INotificationBehavior<>` implementations are auto-registered under their interfaces (transient). Explicitly registered behaviors (`AddDecoration`, including generic closures — `DSAG010` notes the generic ones aren't auto-registered) resolve through the same channel.
- **Fast dictionary lookups for object dispatch** — `Send(object)` / `Publish(object)` use `typeof` chains in small projects and static dictionaries once the project defines more than 8 messages; unknown types throw `HandlerNotFoundException`.
- **Both `IDispatcher` and the concrete `Mediator` are usable** — the concrete class is the fastest path.
- **Compile-time message diagnostics** (`DSAG006`–`DSAG010`) — ambiguous message contracts, missing handlers, unsupported message shapes, and generic behaviors are reported at build time.

```csharp
// Inject the concrete generated mediator for monomorphized dispatch.
var mediator = provider.GetRequiredService<RA.Utilities.Dispatcher.Generated.Mediator>();
Result<Pong> pong = await mediator.Send(new Ping("hello"));
Result<object?> pongObject = await mediator.Send((object)new Ping("hello"));
```

See the [generated mediator documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/Application/Dispatcher/generated-mediator/) for the dispatch matrix and diagnostics.

### 4. Base Handlers


Abstract base classes that implement the `IRequestHandler` interfaces. Inherit from these to focus on business logic without boilerplate.

| Base Class | Interface Implemented | Use Case |
|---|---|---|
| `RequestHandler<TRequest>` | `IRequestHandler<TRequest>` | Commands with no return value |
| `RequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResponse>` | Commands/queries that return data |

Each base class exposes a **public abstract** `HandleAsync` method — `ValueTask<Result> HandleAsync(TRequest, CancellationToken)` or `ValueTask<Result<TResponse>> HandleAsync(TRequest, CancellationToken)` — plus a **protected virtual** context-aware overload that delegates to it. Return `Result.Success(...)` / `Result.Failure(exception)` instead of throwing for expected failures.

Namespace: `RA.Utilities.Dispatcher.Handlers`

### 5. Pipeline Behaviors

Pipeline behaviors wrap request handlers to add cross-cutting concerns. They implement `IPipelineBehavior<TRequest>` or `IPipelineBehavior<TRequest, TResponse>` and are composed into a chain via the mediator.

**Built-in request pipeline behaviors:**

| Behavior | Description |
|---|---|
| `LoggingBehavior<TRequest>` / `LoggingBehavior<TRequest, TResponse>` | Logs each request and its response at `Information` level |
| `ValidationBehavior<TRequest>` / `ValidationBehavior<TRequest, TResponse>` | Executes FluentValidation validators; returns a failed `Result` containing a `BadRequestException` on validation failure |

Register per-feature via the fluent builder:

```csharp
builder.Services
    .AddDispatcher<MyCommand, int, MyCommandHandler>()
    .AddDecoration<LoggingBehavior<MyCommand, int>>()
    .AddValidator<MyCommandValidator>();
```

### 6. Notification System

Publish notifications to zero or more handlers. Each handler is wrapped in its own notification behavior pipeline and returns a `ValueTask<Result>`; `Publish` runs every handler and aggregates failures into the returned `Result` (a single exception when one handler fails, an `AggregateException` when several fail).

| Abstraction | Role |
|---|---|
| `INotification` | Marker interface for notification types |
| `INotificationHandler<TNotification>` | Handles a notification |
| `INotificationBehavior<TNotification>` | Cross-cutting concerns for notification handlers |

**Built-in notification behaviors:**

| Behavior | Description |
|---|---|
| `NotificationLoggingBehavior<TNotification>` | Logs each notification at start and finish |
| `NotificationMetricsBehavior<TNotification>` | Measures handler duration; warns if over 500 ms |
| `NotificationRetryBehavior<TNotification>` | Retries failed handlers up to N times with configurable backoff |

Notification **handlers** are discovered by the source generator automatically. Add notification **behaviors** via the fluent builder:

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();
```

### 7. Fluent Validation Integration

The `ValidationBehavior` automatically discovers and executes all registered `IValidator<TRequest>` implementations. If validation fails, the behavior returns `Result.Failure(...)` containing a `BadRequestException` built from the collected `ValidationFailure` entries (via `ValidationUtilities.CreateValidationErrorResult`) — invalid data never reaches your handler, and the caller can map the failed result to a `400 Bad Request` response.

### 8. Pipeline Context

The `PipelineContext<T>` provides a **strongly-typed** data carrier that flows through the entire pipeline. Each `Send` or `Publish` call gets its own isolated instance. Behaviors and handlers read and write properties on the user-defined `T` — no dictionaries, no magic strings, no boxing.

```csharp
// Define your context type
public class MyPipelineContext
{
    public string? CorrelationId { get; set; }
    public int? UserId { get; set; }
    public Stopwatch? Timer { get; set; }
}

// A context-aware behavior that stamps a correlation ID
public class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public ValueTask<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        => HandleAsync(request, _ => next(), new PipelineContext<MyPipelineContext>(), ct);

    public async ValueTask<Result<TResponse>> HandleAsync<TContext>(
        TRequest request, RequestHandlerContextDelegate<TResponse, TContext> next,
        PipelineContext<TContext> context, CancellationToken ct)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyPipelineContext> ctx)
            ctx.Data.CorrelationId = Guid.NewGuid().ToString();
        return await next(context);
    }
}

// A handler that reads the correlation ID
public class MyHandler : RequestHandler<MyCommand, Data>
{
    protected override async ValueTask<Result<Data>> HandleAsync<TContext>(
        MyCommand request, PipelineContext<TContext> context, CancellationToken ct)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyPipelineContext> ctx)
            Console.WriteLine($"CorrelationId: {ctx.Data.CorrelationId}");
        return await base.HandleAsync(request, context, ct);
    }
}

// Caller provides (or omits) context
var ctx = new PipelineContext<MyPipelineContext>();
ctx.Data.UserId = 42;
Result<Data> result = await mediator.Send<MyCommand, Data, MyPipelineContext>(command, ctx);
```

The `IDispatcher` interface has overloads that accept a context type parameter:
- `Send<TRequest, TResponse, TContext>(request, context?, ct)` — request/response with typed context
- `Send<TRequest, TContext>(request, context?, ct)` — request without response, with typed context
- `Publish<TNotification, TContext>(notification, context?, ct)` — notification with typed context

Existing `Send<TRequest, TResponse>(...)` and `Publish<TNotification>(...)` calls continue to work unchanged — the context type parameter is entirely opt-in.

---

## 🚀 Usage Example — Request/Response

Let's walk through creating a complete feature slice for creating a new product.

### Step 1: Define the Command and Validator

```csharp
// Features/Products/CreateProduct.cs

using FluentValidation;
using RA.Utilities.Dispatcher.Abstractions;

// The command containing the data for the new product.
// TResponse is the payload; the dispatcher wraps it in Result<TResponse>.
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;

// The validator for the command
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### Step 2: Implement the Handler

Inherit from `RequestHandler<TRequest, TResponse>` and return a `ValueTask<Result<TResponse>>`. Return `Result.Failure` with a typed exception for expected failures; exceptions remain available for truly exceptional cases.

```csharp
// Features/Products/CreateProduct.cs (continued)
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Handlers;

public class CreateProductHandler : RequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public override async ValueTask<Result<int>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (await _productRepository.DoesProductExistAsync(command.Name))
        {
            return Result.Failure<int>(new ConflictException(nameof(Product), command.Name));
        }

        var newProduct = new Product { Name = command.Name, Price = command.Price };
        int productId = await _productRepository.AddAsync(newProduct);

        return Result.Success(productId);
    }
}
```

### Step 3: Register Services in `Program.cs`

The handler above is discovered by the source generator, so **no registration call is needed for it**. Only the validator (which also wires up the `ValidationBehavior`) still requires explicit registration:

```csharp
// Program.cs
using RA.Utilities.Dispatcher.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator(); // registers IDispatcher + all auto-discovered handlers

builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>();

var app = builder.Build();

app.MapEndpoints(app.Services);

app.Run();
```

`AddDispatcher` re-registers the handler (harmless — same scoped registration), so you only need it when the feature also has behaviors or validators to add.

---

## 📬 Usage Example — Notifications

Publish a notification from a request handler, and let multiple handlers process it independently.

### Step 1: Define the Notification

```csharp
// Features/Orders/OrderPlaced.cs

using RA.Utilities.Dispatcher.Abstractions;

public record OrderPlaced(int OrderId, string CustomerEmail) : INotification;
```

### Step 2: Implement Handlers

```csharp
// Features/Orders/SendConfirmationEmail.cs
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;

public class SendConfirmationEmail : INotificationHandler<OrderPlaced>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendConfirmationEmail> _logger;

    public SendConfirmationEmail(IEmailService emailService, ILogger<SendConfirmationEmail> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async ValueTask<Result> HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(notification.CustomerEmail, "Order Confirmed", /* ... */);
        _logger.LogInformation("Confirmation email sent for order {OrderId}", notification.OrderId);
        return Result.Success();
    }
}

// Features/Orders/UpdateInventory.cs
public class UpdateInventory : INotificationHandler<OrderPlaced>
{
    public async ValueTask<Result> HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
    {
        // reduce stock levels...
        return Result.Success();
    }
}
```

### Step 3: Publish

Notification handlers are auto-registered by the generator, so nothing needs to be added in `Program.cs` for plain handlers. Add notification **behaviors** explicitly when you need them:

```csharp
// Program.cs — notification behaviors only (handlers are auto-registered)
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();

// Inside a request handler or endpoint — publishing
Result publishResult = await mediator.Publish(new OrderPlaced(orderId, customerEmail), cancellationToken);
```

---

## 🧩 Bringing It All Together

A complete `Program.cs` might look like:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register the dispatcher — also applies all compile-time handler registrations
builder.Services.AddMediator();

// 2. Wire validators and behaviors per feature (handlers themselves need no registration)
builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>();

builder.Services
    .AddDispatcher<DeleteProductCommand, DeleteProductHandler>()
    .AddValidator<DeleteProductCommandValidator>();

// 3. Notification behaviors (handlers are auto-registered)
builder.Services
    .AddNotification<OrderPlaced>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();

var app = builder.Build();
app.MapEndpoints(app.Services);
app.Run();
```

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it.

### Pull Request Process

1.  **Fork the Repository**: Start by forking the RA.Utilities repository.
2.  **Create a Branch**: Create a new branch for your feature or bug fix from the `main` branch.
3.  **Make Your Changes**: Write your code, ensuring it adheres to the existing coding style. Add or update XML documentation for any new public APIs.
4.  **Update README**: If you are adding new functionality, please update the `README.md` file accordingly.
5.  **Submit a Pull Request**: Push your branch to your fork and open a pull request to the `main` branch of the original repository. Provide a clear description of the changes you have made.

### Coding Standards

-   Follow the existing coding style and conventions used in the project.
-   Ensure all public members are documented with clear XML comments.
-   Keep changes focused. A pull request should address a single feature or bug.

Thank you for contributing!
