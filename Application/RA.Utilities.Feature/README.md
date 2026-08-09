# RA.Utilities.Feature

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Feature.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Feature/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Feature.svg)](https://www.nuget.org/packages/RA.Utilities.Feature/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Application/Feature/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)


`RA.Utilities.Feature` provides a foundational toolkit for implementing the **Vertical Slice Architecture** pattern using CQRS (Command Query Responsibility Segregation).
It includes a custom mediator, base handlers, pipeline behaviors for cross-cutting concerns, a notification system, and seamless integration with the `Result<T>` type to streamline feature development and promote clean, maintainable code.

Building applications with a traditional layered architecture can lead to wide, coupled classes and scattered logic.
The Vertical Slice pattern, combined with CQRS, addresses this by organizing code around features.
This package provides the essential building blocks to support that pattern.

## Getting started

```bash
dotnet add package RA.Utilities.Feature
```

---

## 🔗 Dependencies

-   [`RA.Utilities.Core`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core/)
-   [`RA.Utilities.Core.Exceptions`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Exceptions/)
-   [`RA.Utilities.Application.Validation`](https://redonalla.github.io/RA.Utilities/nuget-packages/application/RA.Utilities.Application.Validation/)
-   [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/)
-   [`Microsoft.Extensions.DependencyInjection.Abstractions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection)
-   [`Microsoft.Extensions.Logging.Abstractions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.abstractions)

---

## ✨ Features

### 1. Custom Mediator

The package provides its own lightweight **`IMediator`** / **`Mediator`** implementation — no external MediatR dependency. It supports:

- **Request/Response** dispatch with a composable pipeline of behaviors
- **Notification** publishing to zero or more handlers, each wrapped in its own behavior pipeline
- Behavior ordering: behaviors execute in registration order, outermost first

Register the mediator once at startup:

```csharp
builder.Services.AddMediator();
```

### 2. Base Handlers

Abstract base classes that implement the `IRequestHandler` interfaces, providing built-in logging and automatic exception-to-`Result` conversion. Inherit from these to focus on business logic without boilerplate.

| Base Class | Interface Implemented | Use Case |
|---|---|---|
| `RequestHandler<TRequest>` | `IRequestHandler<TRequest>` | Commands with no return value |
| `RequestHandler<TRequest, TResponse>` | `IRequestHandler<TRequest, TResponse>` | Commands/queries that return data |

Both base classes log the start and end of each request via `ILogger`, catch unhandled exceptions, and wrap them in `Result.Failure` — no `try-catch` blocks needed in your handlers.

Namespace: `RA.Utilities.Feature.Handlers`

### 3. Pipeline Behaviors

Pipeline behaviors wrap request handlers to add cross-cutting concerns. They implement `IPipelineBehavior<TRequest>` or `IPipelineBehavior<TRequest, TResponse>` and are composed into a chain via the mediator.

**Built-in request pipeline behaviors:**

| Behavior | Description |
|---|---|
| `LoggingBehavior<TRequest>` / `LoggingBehavior<TRequest, TResponse>` | Logs each request and its result at `Information` level |
| `ValidationBehavior<TRequest>` / `ValidationBehavior<TRequest, TResponse>` | Executes FluentValidation validators; short-circuits with a `BadRequestException` on failure |

Register per-feature via the fluent builder:

```csharp
builder.Services
    .AddFeature<MyCommand, Result<int>, MyCommandHandler>()
    .AddDecoration<LoggingBehavior<MyCommand, Result<int>>>()
    .AddValidator<MyCommandValidator>();
```

### 4. Notification System

Publish fire-and-forget notifications to zero or more handlers. Each handler is wrapped in its own notification behavior pipeline, and one handler's failure does not prevent others from executing.

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

Register notifications via the fluent builder:

```csharp
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();
```

### 5. Fluent Validation Integration

The `ValidationBehavior` automatically discovers and executes all registered `IValidator<TRequest>` implementations. If validation fails, the pipeline short-circuits and returns a `Result.Failure` containing a `BadRequestException` with the validation errors — invalid data never reaches your handler.

### 6. Pipeline Context

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
    public Task<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        => HandleAsync(request, _ => next(), new PipelineContext<MyPipelineContext>(), ct);

    public async Task<Result<TResponse>> HandleAsync<TContext>(
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
public class MyHandler : RequestHandler<MyCommand, Result<Data>>
{
    public MyHandler(ILogger<MyHandler> logger) : base(logger) { }

    protected override async Task<Result<Data>> HandleAsync<TContext>(
        MyCommand request, PipelineContext<TContext> context, CancellationToken ct)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyPipelineContext> ctx)
            _logger.LogInformation("CorrelationId: {Id}", ctx.Data.CorrelationId);
        return await base.HandleAsync(request, context, ct);
    }
}

// Caller provides (or omits) context
var ctx = new PipelineContext<MyPipelineContext>();
ctx.Data.UserId = 42;
var result = await mediator.Send<MyCommand, Result<Data>, MyPipelineContext>(command, ctx);
```

The `IMediator` interface has overloads that accept a context type parameter:
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
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Abstractions;

// The command containing the data for the new product
public record CreateProductCommand(string Name, decimal Price) : IRequest<Result<int>>;

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

Inherit from `RequestHandler<TRequest, TResponse>` to get automatic logging and exception handling.

```csharp
// Features/Products/CreateProduct.cs (continued)
using Microsoft.Extensions.Logging;
using RA.Utilities.Core.Results;
using RA.Utilities.Feature.Handlers;

public class CreateProductHandler : RequestHandler<CreateProductCommand, Result<int>>
{
    private readonly IProductRepository _productRepository;

    public CreateProductHandler(IProductRepository productRepository, ILogger<CreateProductHandler> logger)
        : base(logger)
    {
        _productRepository = productRepository;
    }

    public override async Task<Result<int>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (await _productRepository.DoesProductExistAsync(command.Name))
        {
            return new ConflictException(nameof(Product), command.Name);
        }

        var newProduct = new Product { Name = command.Name, Price = command.Price };
        var productId = await _productRepository.AddAsync(newProduct);

        return productId;
    }
}
```

### Step 3: Register Services in `Program.cs`

```csharp
// Program.cs
using RA.Utilities.Feature.Behaviors;
using RA.Utilities.Feature.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediator();

builder.Services
    .AddFeature<CreateProductCommand, Result<int>, CreateProductHandler>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, Result<int>>>()
    .AddValidator<CreateProductCommandValidator>();

var app = builder.Build();

app.MapEndpoints(app.Services);

app.Run();
```

---

## 📬 Usage Example — Notifications

Publish a notification from a request handler, and let multiple handlers process it independently.

### Step 1: Define the Notification

```csharp
// Features/Orders/OrderPlaced.cs

using RA.Utilities.Feature.Abstractions;

public record OrderPlaced(int OrderId, string CustomerEmail) : INotification;
```

### Step 2: Implement Handlers

```csharp
// Features/Orders/SendConfirmationEmail.cs
using Microsoft.Extensions.Logging;
using RA.Utilities.Feature.Abstractions;

public class SendConfirmationEmail : INotificationHandler<OrderPlaced>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendConfirmationEmail> _logger;

    public SendConfirmationEmail(IEmailService emailService, ILogger<SendConfirmationEmail> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
    {
        await _emailService.SendAsync(notification.CustomerEmail, "Order Confirmed", /* ... */);
        _logger.LogInformation("Confirmation email sent for order {OrderId}", notification.OrderId);
    }
}

// Features/Orders/UpdateInventory.cs
public class UpdateInventory : INotificationHandler<OrderPlaced>
{
    public async Task HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
    {
        // reduce stock levels...
    }
}
```

### Step 3: Register and Publish

```csharp
// Program.cs — registration
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>()
    .AddDecoration<NotificationLoggingBehavior<OrderPlaced>>();

// Inside a request handler or endpoint — publishing
await mediator.Publish(new OrderPlaced(orderId, customerEmail), cancellationToken);
```

---

## 🧩 Bringing It All Together

A complete `Program.cs` might look like:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Register the mediator
builder.Services.AddMediator();

// 2. Register features with validation and logging
builder.Services
    .AddFeature<CreateProductCommand, Result<int>, CreateProductHandler>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, Result<int>>>()
    .AddValidator<CreateProductCommandValidator>();

builder.Services
    .AddFeature<DeleteProductCommand, DeleteProductHandler>()
    .AddValidator<DeleteProductCommandValidator>();

// 3. Register notifications
builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddHandler<UpdateInventory>()
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
