# RA.Utilities.Feature Release Notes

## Version 10.0.2
![Date Badge](https://img.shields.io/badge/Publish-09%20August%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.2-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Feature/10.0.2)

### ✨ New Features

*   **`RequestHandler` Base Classes**:
    *   New `RequestHandler<TRequest>` and `RequestHandler<TRequest, TResponse>` abstract base classes in the `RA.Utilities.Feature.Handlers` namespace.
    *   Provide built-in `ILogger`-based logging for the start and finish of each request.
    *   Automatically catch unhandled exceptions and wrap them in `Result.Failure` — no boilerplate `try-catch` blocks needed in derived handlers.
    *   Derived classes override `HandleAsync` and focus solely on business logic.

*   **Configurable Notification Retry**:
    *   `NotificationRetryBehavior<TNotification>` now accepts `maxRetries` and `baseDelayMilliseconds` via its constructor (defaults: 3 retries, 200 ms base delay).
    *   Parameters include validation guards (`ArgumentOutOfRangeException` on invalid values).

### 🐛 Bug Fixes

*   **Mediator — removed dead code**: The null-coalescing `?? throw` after `GetRequiredService<T>()` was unreachable (`GetRequiredService` never returns null). Removed from both `Send` overloads.
*   **Mediator — per-handler exception isolation in `Publish`**: Previously, one failing notification handler would prevent all subsequent handlers from executing. Each handler is now wrapped in its own `try-catch`; failures are logged and execution continues to the next handler.
*   **Mediator — short-circuit empty handlers in `Publish`**: When no handlers are registered for a notification type, `Publish` returns immediately instead of needlessly resolving notification behaviors.
*   **`NotificationMetricsBehavior` — thread-safety**: Replaced the `Stopwatch` instance field with a local `Stopwatch.StartNew()` variable, eliminating a potential thread-safety hazard if the behavior is ever resolved concurrently.
*   **`ValidationBehavior` — null guard**: Both constructors now guard against `null` validators with `?? Array.Empty<IValidator<TRequest>>()`, preventing a `NullReferenceException` in edge cases (e.g., manual instantiation in tests).
*   **`LoggingBehavior<TRequest>` — consistent log messages**: The no-response variant now mirrors the two-param variant: `"Start. Request: {...}"` and `"Finished. Result: {...}"` instead of the previous inconsistent messages.
*   **`IMediator` — typo fix**: Corrected `"reques"` → `"request"` in XML documentation.

### 📖 Documentation

*   **README overhaul**: Rewritten with comprehensive sections covering the custom mediator, base handlers, pipeline behaviors, the notification system, and all built-in behaviors. Usage examples now include a complete notifications walkthrough in addition to the existing request/response example.
*   Fixed stale code samples (incorrect generic arity in `LoggingBehavior` registration, MediatR references replaced with the correct custom mediator API).

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-12%20January%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Feature/10.0.1)

Add `MustMatchesCurrencyFormat` extension method. This design allows developers to chain this custom validation rule into any string property within their [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) validator classes.

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-23%20November%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Feature/10.0.0)

This change updates the project version from `10.0.0-rc.2` to `10.0.0`, marking the transition from a release candidate to a stable release. This indicates that the application is now considered stable and ready for production use after completing testing and validation of the previous release candidate version.

## Version 10.0.0-rc.2
![Date Badge](https://img.shields.io/badge/Publish-18%20Octomber%202025-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-10.0.0--rc.2-orange?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Feature/10.0.0-rc.2)

This release modernizes the `RA.Utilities.Feature` package, providing a foundational toolkit for implementing the **Vertical Slice Architecture** pattern using CQRS. It offers base handlers, validation behaviors, and seamless integration with the `Result<T>` type to streamline feature development.

### ✨ New Features & Improvements

*   **Base Handlers for CQRS**:
    *   Provides abstract base classes (`BaseHandler<TRequest, TResponse>`) that encapsulate common logic like logging and exception handling.
    *   Handlers automatically catch exceptions and wrap them in a `Result.Failure`, ensuring robust error handling without boilerplate `try-catch` blocks.

*   **Automatic Validation Pipeline Behavior**:
    *   Includes a `ValidationBehavior<TRequest, TResponse>` for MediatR pipelines.
    *   Automatically intercepts incoming requests, finds the corresponding `FluentValidation` validator, and executes it.
    *   If validation fails, the pipeline is short-circuited, and a `Result.Failure` containing a `ValidationException` is returned immediately, preventing invalid data from reaching your business logic.

*   **Seamless `Result<T>` Integration**:
    *   Designed from the ground up to work with the `Result<T>` type from `RA.Utilities.Core`, promoting explicit and predictable error handling for business logic failures.

*   **Updated Documentation**:
    *   The `README.md` has been updated to provide a clear, step-by-step guide for creating a complete feature slice, including the command, validator, handler, and DI registration.

### 🚀 Getting Started

Register MediatR, the validation behavior, and your validators in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Add MediatR and register handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 2. Add the validation pipeline behavior from this package
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// 3. Scan and register all FluentValidation validators
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
```