---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Options
```

`DispatcherOptions` configures the [`AddDispatcher(Action<DispatcherOptions>)`](../Extensions/DispatcherServiceCollectionExtensions.md) overload. It does two things:

- **Filters** the registrations produced by the source generator **before they take effect** — the generated module initializer still queues everything, and the filter removes the descriptors that belong to excluded handlers (and to the pipeline/notification behaviors related to their messages).
- **Registers additional pipeline behaviors explicitly** — including open generic behaviors, which the generator cannot auto-register.

Nothing is filtered by default: when both filter properties are `null`, every discovered handler and closed behavior registers exactly as described in [Auto Registration](../auto-registration). `Behaviors` is empty by default, so no extra behavior is registered.

## 📦 Type Definition

```csharp
public sealed class DispatcherOptions
{
    public List<Type>? IncludeHandlers { get; set; }
    public List<Type>? ExcludeHandlers { get; set; }
    public List<Type> Behaviors { get; }
    public ServiceLifetime BehaviorLifetime { get; set; }
}
```

## 🎯 Member Breakdown

| Property | Type | Default | Description |
|---|---|---|---|
| `IncludeHandlers` | `List<Type>?` | `null` (ignored) | When set, only the listed handlers — and the pipeline/notification behaviors related to the messages they handle — are registered. |
| `ExcludeHandlers` | `List<Type>?` | `null` (ignored) | When set, the listed handlers are not registered, and neither are the behaviors related to a message once no other registered handler handles it. |
| `Behaviors` | `List<Type>` | empty list | Pipeline behaviors to register explicitly, in chain order: the first entry is the outermost behavior. Accepts `IPipelineBehavior<>`, `IPipelineBehavior<,>`, and `INotificationBehavior<>` implementations, closed or open generic. |
| `BehaviorLifetime` | `ServiceLifetime` | `Transient` | The lifetime used for every type in `Behaviors`. |

### Filtering rules

- **Exclusions win** over inclusions when a type is in both lists.
- **Behavior types are accepted** in either list. A listed behavior is kept even when the handler of its message is filtered out (`IncludeHandlers`), and removed even when its handler is kept (`ExcludeHandlers`).
- A **request behavior** is removed together with the handler of its request. A behavior whose request has no handler in the compilation at all is left alone unless `IncludeHandlers` restricts registration.
- A **notification behavior** is removed only once no registered handler for its notification remains.
- The filter applies to **source-generated registrations only**. Handlers registered explicitly with `AddDispatcher<TRequest, TResponse, THandler>()` / `AddNotification<TNotification>()`, behaviors added with `AddDecoration<TBehavior>()`, and behaviors listed in `Behaviors` are untouched.
- Include and exclude are evaluated against the **handler's implementation type**, not the closed interface type. Registering the same handler twice (automatically and explicitly) leaves the explicit registration in place.

## 🧩 Registering pipeline behaviors in bulk

`Behaviors` covers the cases the generator cannot: **open generic** behaviors, behaviors shared by many requests, and behaviors that must wrap the whole auto-registered block.

```csharp showLineNumbers
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Extensions;

builder.Services.AddDispatcher(options =>
{
    // Open generic: registered once and applied to every matching request.
    options.Behaviors.Add(typeof(MetricsBehavior<,>));   // outermost
    options.Behaviors.Add(typeof(LoggingBehavior<,>));   // runs inside metrics

    // Closed: applies to one request/response pair.
    options.Behaviors.Add(typeof(CachingBehavior<GetOrderQuery, OrderDto>));

    // Optional; Transient is the default.
    options.BehaviorLifetime = ServiceLifetime.Transient;
});
```

Rules for the list:

- **Supported contracts** — `IPipelineBehavior<TRequest>`, `IPipelineBehavior<TRequest, TResponse>`, and `INotificationBehavior<TNotification>`. A type implementing several of them is registered under each.
- **Chain order is list order** — the first entry wraps the rest. Configured behaviors are registered **before** the generated registrations, so they form the outer layer around auto-registered closed behaviors; the [`[PipelineBehaviorOrder]`](../Attributes/PipelineBehaviorOrderAttribute.md) attribute is **not** read for them, exactly as with `AddDecoration`.
- **Open generics must pass their own type parameters in order.** `MyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>` works; a partially closed mapping such as `MyBehavior<T> : IPipelineBehavior<ConcreteRequest, T>` is rejected at startup because the container cannot close it.
- **Registered with `TryAddEnumerable`** — listing the same behavior twice, or calling `AddDispatcher` more than once, leaves a single registration. A closed behavior that the generator also auto-registers is registered **once**, at its configured (outer) position; the generated duplicate is removed.
- **Validated at startup** — a non-class, abstract, or non-behavior type throws `ArgumentException` when `AddDispatcher` runs.
- **Transient is recommended** for behaviors because they should not hold request-scoped state. The lifetime is configurable for advanced scenarios.

## 🚀 Complete Example

```csharp showLineNumbers
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Options;

// Register every discovered handler except the legacy importer, only the
// order slice in a trimmed-down worker host, plus two generic behaviors:
builder.Services.AddDispatcher(options =>
{
    options.IncludeHandlers = [typeof(CreateOrderHandler), typeof(GetOrderQueryHandler)];
    options.ExcludeHandlers = [typeof(LegacyImportHandler)];

    options.Behaviors.Add(typeof(MetricsBehavior<,>));
    options.Behaviors.Add(typeof(LoggingBehavior<,>));
});

// Default: register everything the generator discovered, no extra behaviors.
builder.Services.AddDispatcher();
```

With the configuration above, `CreateOrderHandler` and `GetOrderQueryHandler` (plus the behaviors bound to their messages) register; `LegacyImportHandler` and any handler not listed do not. The two generic behaviors still register — filtering never removes explicit registrations. A behavior listed in `ExcludeHandlers` is removed even if its message's handler survives.

## 🔗 Related

- [Auto Registration](../auto-registration) — discovery rules, lifetimes, and the assembly-loading guarantee.
- [Filtering auto-registered handlers](../auto-registration#filtering-auto-registered-handlers) — the same rules in guide form.
- [`DispatcherBuilder.AddDecoration`](../Builders/DispatcherBuilder.md) — attach a behavior to a single request.
- [`DispatcherServiceCollectionExtensions`](../Extensions/DispatcherServiceCollectionExtensions.md) — the `AddDispatcher` overloads that accept these options.
