---
title: RA.Utilities.Dispatcher
authors: [RedonAlla]
date: 2026-10-19
---

## Version 10.0.1
![Date Badge](https://img.shields.io/badge/Publish-19%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.1-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.0.1)

Maintenance release updating the `Microsoft.Extensions.*` dependencies to the latest .NET 10 servicing patch. No API or behavior changes.

<!-- truncate -->

### 📝 Improvements

*   **Dependency updates**: `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.12 → 10.0.13 and `Microsoft.Extensions.Logging.Abstractions` 10.0.12 → 10.0.13. Dependency versions are now managed as floating versions, so every release is built against — and ships with — the latest stable versions.

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-18%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.0.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.0.0)

Initial release of the **`RA.Utilities.Dispatcher`** package — the successor of `RA.Utilities.Feature`. It keeps the source-generated mediator, auto-registration, pipeline behaviors, and notification system, and returns to **Result-based dispatch** with a modern shape.

### 💥 Breaking Changes (from RA.Utilities.Feature 10.x)

*   **Package and namespace rename.** `RA.Utilities.Feature` → `RA.Utilities.Dispatcher`; every namespace moves from `RA.Utilities.Feature.*` to `RA.Utilities.Dispatcher.*`. `IMediator` → `IDispatcher`, `AddFeature` → `AddDispatcher`, `FeatureBuilder` → `DispatcherBuilder`, `NotificationFeatureBuilder` → `NotificationDispatcherBuilder`, and generator diagnostics move from `FEAG001`–`FEAG010` to `DSAG001`–`DSAG010` (same meanings).
*   **Result-based dispatch redesign.** Handlers return `ValueTask<Result<TResponse>>` (`ValueTask<Result>` for void requests), and `IDispatcher.Send` returns `ValueTask<Result<TResponse>>` / `ValueTask<Result>`. Expected failures are **returned** as `Result.Failure(exception)` instead of being thrown — `ValidationBehavior` returns a failed result containing a `BadRequestException` instead of throwing it. Truly exceptional cases still throw and propagate to the caller.
*   **`INotificationHandler` returns a result.** `HandleAsync` returns `ValueTask<Result>` (was `Task` in Feature), and `Publish` aggregates handler failures into the returned `Result` — a single exception when one handler fails, an `AggregateException` when several fail.

<!-- truncate -->

### ✨ New Features

*   **Source-generated `IDispatcher` implementation (`Mediator`).** The source generator emits the complete `IDispatcher` implementation — the **only** dispatcher in the package — into every assembly referencing it, and `AddMediator()` registers it automatically.
    *   **Direct handler injection.** The generated `Send` knows each message's handler, resolves it **by concrete type**, and calls it directly — no interface resolution, no `GetServices` enumeration, and the call devirtualizes.
    *   **No delegate chain without behaviors.** Requests with an empty pipeline invoke the handler directly — nothing is allocated for pipeline composition.
    *   **Closed behaviors auto-register.** Non-generic `IPipelineBehavior<,>` / `IPipelineBehavior<>` / `INotificationBehavior<>` implementations are auto-registered under their interfaces (transient) and injected directly in deterministic order. Generic behaviors report the new `DSAG010` warning and remain available through explicit `AddDecoration` registration.
    *   **Fast dictionary lookups for object dispatch.** `Send(object)` / `Publish(object)` use `typeof` chains in small projects and switch to static `Dictionary<Type, Delegate>` lookups above a project-size threshold (more than 8 messages); unknown message types throw `RA.Utilities.Dispatcher.Exceptions.HandlerNotFoundException`.
*   **Zero-configuration handler registration.** A module initializer per assembly queues DI registrations (request handlers **scoped**, notification handlers **transient**) on `RA.Utilities.Dispatcher.Generated.HandlerRegistrations`; `AddMediator()` applies them in one pass — plain handlers need no explicit registration at all.
*   **Typed `PipelineContext<T>`.** Strongly-typed data carrier flowing through the entire pipeline — context-aware `Send`/`Publish` overloads, context-aware delegate types, and default interface methods keep existing calls compiling unchanged.
*   **Abstract `RequestHandler` base classes.** `RequestHandler<TRequest>` and `RequestHandler<TRequest, TResponse>` — pure passthroughs exposing one public abstract `HandleAsync` plus a protected virtual context-aware overload.
*   **Built-in behaviors.** `LoggingBehavior` (request logging at Information/Warning), `ValidationBehavior` (FluentValidation, returns a failed result carrying `BadRequestException`), `NotificationLoggingBehavior`, `NotificationMetricsBehavior` (allocation-free timestamp, warns over 500 ms), and `NotificationRetryBehavior` (configurable retries with incremental backoff).
*   **C# 14 extension-block registration API.** `AddMediator`, `AddDispatcher`, and `AddNotification` are implemented as C# 14 extension members — identical call sites, modern syntax.
*   **Compile-time message diagnostics** (category `RA.Utilities.Dispatcher.Generators`): `DSAG001`–`DSAG005` (registration validity), `DSAG006`–`DSAG009` (message/handler contracts), `DSAG010` (generic behaviors) — ambiguous contracts, missing handlers, and unsupported shapes are reported at build time.

### ⚡ Performance

Benchmarks (MediatR handlers registered scoped, matching this package's lifetime; Apple M1, .NET 10.0.11, BenchmarkDotNet 0.15.8, MediatR 14.2.0, 2026-10-18):

| Scenario | MediatR | Generated `Mediator` (concrete) | Delta |
|---|---|---|---|
| Send, 0 behaviors | 283.2 ns / 608 B | **218.4 ns / 464 B** | −23% time, −24% alloc |
| Send, 3 behaviors | 419.2 ns / 1136 B | **327.8 ns / 960 B** | −22% time, −15% alloc |
| Send, void | 245.9 ns / 512 B | **215.2 ns / 432 B** | −12% time, −16% alloc |
| Send, void + 3 behaviors | 419.6 ns / 1040 B | **317.8 ns / 928 B** | −24% time, −11% alloc |
| Publish, 1 handler | 410.1 ns / 968 B | **295.2 ns / 672 B** | −28% time, −31% alloc |
| Publish, 3 handlers | 639.3 ns / 1448 B | **390.2 ns / 992 B** | −39% time, −31% alloc |

The concrete generated mediator is **12–39% faster than MediatR with 11–31% fewer allocations** in every comparable shape, with the biggest wins on notification fan-out. The interface-typed `IDispatcher` path matches MediatR's speed on request dispatch while allocating less everywhere.

### 📖 Documentation

*   New Docusaurus section under `nuget-packages/Application/Dispatcher` following the project's documentation template: package index, auto-registration guide, generated-mediator guide, benchmarks, migration guide (`RA.Utilities.Feature` 10.x → `RA.Utilities.Dispatcher` 10.0.0), troubleshooting & FAQ, and per-type reference pages for `Abstractions`, `Behaviors`, `Builders`, `Exceptions`, `Extensions`, `Generated`, `Handlers`, and `Models`.
*   New `RA.Utilities.Dispatcher.Benchmarks` BenchmarkDotNet project comparing the generated mediator against MediatR 14.2 on identical send and publish scenarios, with per-scope resolution mirroring per-request semantics.
