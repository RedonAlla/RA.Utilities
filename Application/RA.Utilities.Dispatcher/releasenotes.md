# Release Notes for RA.Utilities.Dispatcher

## Version 10.1.0

![Date Badge](https://img.shields.io/badge/Publish-31%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.1.0)

The generated implementation and the bootstrap call now carry the package name — `Dispatcher` and `AddDispatcher()` — and the package gains registration filtering and deterministic behavior ordering. Most applications change one startup line.

### ⚠️ Breaking Changes

*   **`AddMediator()` → `AddDispatcher()`.** The parameterless bootstrap was renamed. The fluent `AddDispatcher<TRequest, TResponse, THandler>()`, `AddNotification<TNotification>()`, `AddDecoration<TBehavior>()`, and `AddValidator<TValidator>()` APIs are unchanged.
*   **Generated `Mediator` → generated `Dispatcher`.** Code that injects the concrete class must rename the type. Inside a `RA.Utilities.*` namespace, qualify it or alias it (`using GeneratedDispatcher = RA.Utilities.Dispatcher.Generated.Dispatcher;`) — the simple name resolves to the namespace (`CS0118`).
*   **`MediatorRegistrations` → `DispatcherRegistrations`** and **`MediatorServiceCollectionExtensions` → `DispatcherServiceCollectionExtensions`.** Internal plumbing; only relevant if referenced by name.

### ✨ New Features

*   **Handler filtering.** `AddDispatcher(options => ...)` accepts `DispatcherOptions` with `IncludeHandlers` / `ExcludeHandlers` (`List<Type>?`). Filtering applies to source-generated registrations only, both lists accept behavior types, exclusions win over inclusions, and behaviors related to a filtered-out message are removed with it.
*   **Deterministic behavior ordering.** `[PipelineBehaviorOrder(int)]` on a closed behavior pins its position: lower values run closer to the outside, the default is `0`, and ties fall back to the type name. The attribute is read at compile time and only affects auto-registered behaviors; generic behaviors and explicit `AddDecoration` calls keep call order.

### 📝 Improvements

*   Behavior registrations are emitted by message interface, then order, then type name — the chain no longer depends on incidental emission order.
*   Documentation rebuilt around the project template, with new `Options` and `Attributes` reference pages, filtering/ordering guides, name-shadowing guidance, and a v10.0.x → v10.1.0 migration guide.

## Version 10.0.1

![Date Badge](https://img.shields.io/badge/Publish-19%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.0.1)

Maintenance release updating the `Microsoft.Extensions.*` dependencies to the latest .NET 10 servicing patch. No API or behavior changes.

### 📝 Improvements

*   **Dependency updates**: `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.12 → 10.0.13 and `Microsoft.Extensions.Logging.Abstractions` 10.0.12 → 10.0.13. Dependency versions are now managed as floating versions, so every release is built against — and ships with — the latest stable versions.

## Version 10.0.0

![Date Badge](https://img.shields.io/badge/Publish-18%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.0.0)

This is the initial stable release of `RA.Utilities.Dispatcher`, a lightweight, high-performance, source-generated mediator for implementing the Vertical Slice Architecture pattern with CQRS. It combines Result-based dispatch, zero-configuration compile-time handler registration, pipeline behaviors, and an isolated notification system.

### ✨ Key Features

*   **Source-Generated Mediator (`IDispatcher`)**:
    *   Generates a dedicated `Mediator` implementation into referencing assemblies at compile time via Roslyn incremental generators (`RA.Utilities.Dispatcher.Generators`).
    *   **Direct concrete handler injection**: Resolves handlers by their concrete types and invokes them directly with devirtualized calls — avoiding runtime interface resolution and reflection.
    *   **Zero delegate chain**: Requests with no registered behaviors invoke the handler directly with no delegate chain allocations.
    *   **Object dispatch**: High-performance `Send(object)` and `Publish(object)` utilizing `typeof` chains for small projects and static dictionary lookups for larger projects (> 8 messages).

*   **Result-Based Dispatch**:
    *   Handlers and `IDispatcher.Send` return `ValueTask<Result<TResponse>>` (or `ValueTask<Result>` for void requests) powered by `RA.Utilities.Core.Results`.
    *   Expected business and validation failures return `Result.Failure(exception)` rather than throwing exceptions, making failures predictable and allocation-friendly.
    *   `Publish` runs all notification handlers independently and returns an aggregated `Result`.

*   **Zero-Configuration Compile-Time Handler Registration**:
    *   The shipped Roslyn incremental generator discovers all non-abstract classes implementing `IRequestHandler<TRequest, TResponse>`, `IRequestHandler<TRequest>`, or `INotificationHandler<TNotification>`.
    *   Emits a C# module initializer (`[ModuleInitializer]`) that automatically queues DI registrations upon assembly loading.
    *   Calling `services.AddMediator()` registers the mediator and all discovered handlers (scoped for request handlers, transient for notification handlers) with zero reflection at runtime.

*   **Base Request Handlers**:
    *   `RequestHandler<TRequest>` and `RequestHandler<TRequest, TResponse>` in `RA.Utilities.Dispatcher.Handlers`.
    *   Exposes `ValueTask<Result> HandleAsync(TRequest, CancellationToken)` and `ValueTask<Result<TResponse>> HandleAsync(TRequest, CancellationToken)` for business logic.
    *   Provides overridable context-aware overloads accepting typed `PipelineContext<TContext>`.

*   **Pipeline Behaviors**:
    *   `IPipelineBehavior<TRequest, TResponse>` and `IPipelineBehavior<TRequest>` for composable cross-cutting request concerns.
    *   Non-generic pipeline behaviors are auto-discovered and registered transiently; generic behaviors are registered via `.AddDecoration<TBehavior>()`.
    *   **`LoggingBehavior`**: Automatically logs request start and completion with timing and response status.
    *   **`ValidationBehavior`**: Integrates with FluentValidation (`RA.Utilities.Application.Validation`); validates requests before reaching the handler and returns a failed `Result` carrying a `BadRequestException` on validation failure.

*   **Notification System**:
    *   Publish `INotification` messages to zero or more `INotificationHandler<TNotification>` implementations.
    *   Independent execution pipelines per handler with built-in notification behaviors:
        *   `NotificationLoggingBehavior`: Logs notification dispatch lifecycle.
        *   `NotificationMetricsBehavior`: Measures execution duration and warns if handler execution exceeds 500 ms.
        *   `NotificationRetryBehavior`: Retries failed handlers with configurable retry counts and backoff delay.

*   **Strongly-Typed Pipeline Context**:
    *   `PipelineContext<T>` flows user-defined context data through pipeline behaviors and handlers without dictionary lookups, boxing, or magic strings.
    *   Overloads: `Send<TRequest, TContext>`, `Send<TRequest, TResponse, TContext>`, and `Publish<TNotification, TContext>`.

*   **Roslyn Analyzer & Diagnostics (`DSAG001`–`DSAG010`)**:
    *   `DSAG001`: Module initializers unavailable (pre-.NET 5).
    *   `DSAG002`: Generic handler types cannot be auto-registered.
    *   `DSAG003`: Handler types must be accessible (`internal` or `public`).
    *   `DSAG004`: Struct handlers are not supported.
    *   `DSAG005`: Duplicate request handler registration detected.
    *   `DSAG006`: Message implements multiple message contracts.
    *   `DSAG007`: Request message has no handler in compilation.
    *   `DSAG008`: Message falls back to generic dispatch.
    *   `DSAG009`: Handler implements both void and response request contracts for the same message.
    *   `DSAG010`: Generic behavior types are not auto-registered.

### 📦 Dependencies

*   `RA.Utilities.Core.Results`
*   `RA.Utilities.Core.Exceptions`
*   `RA.Utilities.Application.Validation`
*   `Microsoft.Extensions.DependencyInjection.Abstractions`
*   `Microsoft.Extensions.Logging.Abstractions`
