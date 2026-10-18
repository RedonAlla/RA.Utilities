---
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Dispatcher.Behaviors
```

`LoggingBehavior` is a pipeline behavior that logs each request and its outcome. It comes in two variants — one for requests with a response and one for requests without.

## 📦 Type Definition

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger);
}

public class LoggingBehavior<TRequest> : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest>> logger);
}
```

## 🎯 What It Logs

| Event | Level | Message |
|---|---|---|
| Request received | Information | `[Request Logging] Start. Request: {@Request}` |
| Success (with response) | Information | `[Request Logging] Finished. Response: {@Response}` |
| Success (no response) | Information | `[Request Logging] Finished.` |
| Failed result | Warning | `[Request Logging] Failed.` (with the failure exception) |

The behavior wraps `next()` and inspects the returned `Result` — failed results are logged at `Warning` with the exception; the result is passed through unchanged.

## 🚀 Usage

```csharp
builder.Services
    .AddDispatcher<CreateProductCommand, int, CreateProductHandler>()
    .AddDecoration<LoggingBehavior<CreateProductCommand, int>>();
```

Because `LoggingBehavior` is generic, the auto-registration generator reports `DSAG010` and skips it — register it explicitly with `AddDecoration`, as above.

## 🧠 Notes

- Structured logging serializes the full request and response objects (`{@Request}`, `{@Response}`). For high-throughput endpoints with large payloads, consider a custom behavior capturing only essential fields.
- The base `RequestHandler` classes do **not** log — this behavior is the supported way to get request logging.
