---
sidebar_position: 4
---

```powershell
Namespace: RA.Utilities.Dispatcher.Abstractions
```

`INotification` is the marker interface for your **notifications** — the events of your publish/subscribe system. It carries no members; notifications are plain DTOs describing something that happened.

## 📦 Type Definition

```csharp
/// <summary>
/// Represents a notification that can be published and handled by zero or more handlers.
/// </summary>
public interface INotification;
```

## 🚀 Usage

```csharp
public record OrderPlaced(int OrderId, string CustomerEmail) : INotification;

public record UserCreatedEvent(Guid UserId) : INotification;
```

## 🎯 Send vs Publish

| `IRequest<TResponse>` | `INotification` |
|---|---|
| Exactly one handler | Zero or more handlers |
| Caller expects a response | Fire-and-forget side effects |
| `dispatcher.Send(...)` | `dispatcher.Publish(...)` |

Publishing:

```csharp
Result publishResult = await dispatcher.Publish(new OrderPlaced(42, "customer@example.com"), ct);
// Publish runs every handler; failures are aggregated into the returned Result.
```

## 🔗 Related

- [`INotificationHandler<TNotification>`](INotificationHandler.md) — the subscriber contract.
- [`INotificationBehavior<TNotification>`](INotificationBehavior.md) — cross-cutting concerns per notification handler.
