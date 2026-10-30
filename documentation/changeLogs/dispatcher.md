---
title: RA.Utilities.Dispatcher
authors: [RedonAlla]
date: 2026-10-31
---

## Version 10.1.0
![Date Badge](https://img.shields.io/badge/Publish-31%20October%202026-lightblue?logo=fastly&logoColor=white)
[![NuGet version](https://img.shields.io/badge/NuGet-v10.1.0-blue?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Dispatcher/10.1.0)

Completes the package rename — the generated implementation is now `Dispatcher` and the bootstrap is `AddDispatcher()` — and adds registration filtering plus deterministic behavior ordering. Most applications change one startup line; handlers, builders, and behaviors keep their names.

<!-- truncate -->

### ⚠️ Breaking Changes

*   **Generated type renamed `Mediator` → `Dispatcher`.** The source generator now emits `RA.Utilities.Dispatcher.Generated.Dispatcher` and registers it as itself for direct injection. Code that injects the concrete class must rename the type. Inside a `RA.Utilities.*` namespace the simple name resolves to the `RA.Utilities.Dispatcher` namespace (`CS0118`), so alias it: `using GeneratedDispatcher = RA.Utilities.Dispatcher.Generated.Dispatcher;`. Injecting [`IDispatcher`](/nuget-packages/Application/Dispatcher/Abstractions/IDispatcher) is unaffected.
*   **Bootstrap renamed `AddMediator()` → `AddDispatcher()`.** The parameterless overload now carries the package name and replaces `AddMediator()` at startup. Everything else about registration is unchanged — the fluent `AddDispatcher<TRequest, TResponse, THandler>()`, `AddNotification<TNotification>()`, `AddDecoration<TBehavior>()`, and `AddValidator<TValidator>()` APIs keep their names and behavior. The two `AddDispatcher` overloads are distinct methods, so mixing them compiles unchanged. See the [migration guide](/nuget-packages/Application/Dispatcher/migration-guides).
*   **`MediatorRegistrations` → `DispatcherRegistrations`, `MediatorServiceCollectionExtensions` → `DispatcherServiceCollectionExtensions`.** Internal plumbing behind the generator and the bootstrap; only relevant if you referenced the queues or the extension class by name.
*   **Documentation page renamed** — *Generated Mediator* is now *Generated Dispatcher*.

### ✨ New Features

*   **Handler filtering in `AddDispatcher`.** The new overload `AddDispatcher(Action<DispatcherOptions> configure)` accepts a [`DispatcherOptions`](/nuget-packages/Application/Dispatcher/Options/DispatcherOptions) instance with two `List<Type>?` properties:
    *   **`IncludeHandlers`** — when set, only the listed handlers (and the behaviors related to the messages they handle) are registered.
    *   **`ExcludeHandlers`** — when set, the listed handlers are not registered, nor are the behaviors related to their messages once no registered handler handles them.
    *   Exclusions win over inclusions, both lists accept behavior types, and the filter applies to **source-generated registrations only** — handlers registered explicitly with the fluent builders or plain DI stay registered. Lifting the filter means passing `null` (the default) or dropping the options overload.
*   **Deterministic behavior ordering with `[PipelineBehaviorOrder(int)]`.** The new [`PipelineBehaviorOrderAttribute`](/nuget-packages/Application/Dispatcher/Attributes/PipelineBehaviorOrderAttribute) pins the position of an auto-registered closed behavior in its chain. Lower values run closer to the outside (executed earlier), the default is `0`, and ties fall back to the type name. The generator emits behavior registrations sorted by message interface, then order, then type name, and wraps them in reverse — so the first registered behavior remains outermost. The attribute is read at compile time and only affects auto-registered behaviors; generic behaviors (`DSAG010`) and explicit `AddDecoration` calls keep call order.

### 📝 Improvements

*   **Behavior registration order is now explicit and documented.** Closed behaviors still auto-register, but their position no longer depends on the order the generator happens to emit them; `[PipelineBehaviorOrder]` makes the chain independent of type names.
*   **`HandlerRegistrations.ApplyAll` accepts `DispatcherOptions`.** The internal queue drain applies the filter over the descriptors added in its own pass (`[startIndex, services.Count)`), so registrations made elsewhere in the collection are never touched.
*   **Documentation reworked** following the project's documentation template: a new landing page with getting started, core concepts, how-to guides, and configuration; new [`Options`](/nuget-packages/Application/Dispatcher/Options) and [`Attributes`](/nuget-packages/Application/Dispatcher/Attributes) namespaces; filtering and ordering guides; a name-shadowing warning for the generated `Dispatcher` type; updated troubleshooting/FAQ entries for filtering, attribute scope, and `CS0118`; and refreshed migration guides covering v10.0.x → v10.1.0 and `RA.Utilities.Feature` → `RA.Utilities.Dispatcher` 10.0.0.
*   **Test coverage** — ordering tests for send, void send, publish, descriptor order, filter interaction, and explicit decorations before/after `AddDispatcher()`; filtering tests for handler and behavior removal.

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
