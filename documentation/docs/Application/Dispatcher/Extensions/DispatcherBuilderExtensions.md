---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Extensions
```

`DispatcherBuilderExtensions` provides the fluent entry points for configuring request and notification dispatch — `AddDispatcher` and `AddNotification`. It is written with a **C# 14 extension block**, so the methods appear on `IServiceCollection` like classic extension methods.

## 📦 Type Definition

```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Dispatcher/Extensions/DispatcherBuilderExtensions.cs

public static class DispatcherBuilderExtensions
{
    extension(IServiceCollection services)
    {
        // Request with a response
        public DispatcherBuilder<TRequest, TResponse> AddDispatcher<TRequest, TResponse, THandler>()
            where TRequest : IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>;

        // Request without a response
        public DispatcherBuilder<TRequest> AddDispatcher<TRequest, THandler>()
            where TRequest : IRequest
            where THandler : class, IRequestHandler<TRequest>;

        // Notification
        public NotificationDispatcherBuilder<TNotification> AddNotification<TNotification>()
            where TNotification : INotification;
    }
}
```

## 🎯 Method Breakdown

### `AddDispatcher<TRequest, TResponse, THandler>()`

Registers the handler **scoped** and returns a [`DispatcherBuilder<TRequest, TResponse>`](../Builders/DispatcherBuilder.md):

```csharp
services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
```

### `AddDispatcher<TRequest, THandler>()`

The void-request overload — registers `IRequestHandler<TRequest>` scoped and returns a [`DispatcherBuilder<TRequest>`](../Builders/DispatcherBuilder.md).

### `AddNotification<TNotification>()`

Returns a [`NotificationDispatcherBuilder<TNotification>`](../Builders/NotificationDispatcherBuilder.md) for adding handlers and behaviors — it registers nothing by itself.

## 🚀 Usage

```csharp
builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddValidator<CreateProductCommandValidator>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>();

builder.Services
    .AddDispatcher<DeleteProductCommand, DeleteProductHandler>()
    .AddValidator<DeleteProductCommandValidator>();

builder.Services
    .AddNotification<OrderPlaced>()
    .AddHandler<SendConfirmationEmail>()
    .AddDecoration<NotificationRetryBehavior<OrderPlaced>>();
```

:::note
Plain (non-generic) handlers are auto-registered by the package's source generator when `AddMediator()` is called — `AddDispatcher` re-registers the handler (harmless, same scoped registration), so you only need it when the feature also has behaviors or validators to add, or for generic handlers / lazily-loaded assemblies.
:::

## 🔗 Related

- [`MediatorServiceCollectionExtensions`](MediatorServiceCollectionExtensions.md) — the `AddMediator()` bootstrap.
- [`DispatcherBuilder`](../Builders/DispatcherBuilder.md) / [`NotificationDispatcherBuilder`](../Builders/NotificationDispatcherBuilder.md) — the returned builders.
