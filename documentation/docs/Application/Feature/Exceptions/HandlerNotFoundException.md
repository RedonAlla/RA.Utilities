---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Feature.Exceptions
```

`HandlerNotFoundException` is thrown when an **object-based** `Send` or `Publish` call cannot be dispatched because the runtime type of the message is unknown to the generated mediator's dispatch — the type implements no message contract the mediator knows.

## 🎯 Purpose

The object-based dispatch (`Mediator.Send(object)` / `Mediator.Publish(object)`) maps a message's runtime type to its closed handler contract. When the type implements none of the message contracts — or implements an ambiguous one — there is nothing to dispatch, and `HandlerNotFoundException` reports the offending type instead of failing with a generic `InvalidOperationException`.

Common causes:

-   The message type does not implement `IRequest`, `IRequest<TResponse>`, or `INotification`.
-   The message implements **more than one** `IRequest<TResponse>` interface, so its response contract is ambiguous (the compile-time diagnostic `FEAG006` flags the same condition).

## 📦 Type Definition

```csharp showLineNumbers
public class HandlerNotFoundException : Exception
{
    /// <summary>The runtime type of the message that could not be dispatched.</summary>
    public Type MessageType { get; }

    public HandlerNotFoundException(Type messageType);
    public HandlerNotFoundException(Type messageType, string reason);
}
```

The exception message names the message type and — for the ambiguity case — the reason, so the failure is immediately actionable.

## 🚀 Usage Example

```csharp showLineNumbers
try
{
    object? response = await mediator.Send((object)someUnknownMessage);
}
catch (HandlerNotFoundException ex)
{
    Console.WriteLine($"No handler contract for {ex.MessageType.Name}.");
}
```

## 🧠 Summary

`HandlerNotFoundException` is the typed failure for object-based dispatch of messages without a handler contract. Strongly-typed `Send<TRequest, TResponse>` calls are unaffected — they surface missing registrations through the DI container's own exceptions, as before.
