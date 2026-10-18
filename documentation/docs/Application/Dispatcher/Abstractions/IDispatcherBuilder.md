---
sidebar_position: 8
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`IDispatcherBuilder` is the shared contract behind the fluent builders in the [`Builders`](../Builders/index.mdx) namespace. You rarely reference it directly — the concrete builders implement it.

## 📦 Type Definition

```csharp
public interface IDispatcherBuilder
{
    IServiceCollection Services { get; }
}
```

## 🎯 Purpose

The builder pattern needs a common shape so tooling and extension methods can target any dispatch builder — request/response, request-only, or notification — without knowing the concrete generic arity. `IDispatcherBuilder` exposes the single thing every builder holds: the `IServiceCollection` being configured.

## 🏗️ Implementations

| Builder | Purpose |
|---|---|
| [`DispatcherBuilder<TRequest>`](../Builders/DispatcherBuilder.md) | Configures a request without a response |
| [`DispatcherBuilder<TRequest, TResponse>`](../Builders/DispatcherBuilder.md) | Configures a request with a response |
| [`NotificationDispatcherBuilder<TNotification>`](../Builders/NotificationDispatcherBuilder.md) | Configures a notification and its handlers/behaviors |
