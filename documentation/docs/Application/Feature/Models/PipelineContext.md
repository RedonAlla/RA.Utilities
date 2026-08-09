---
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Feature.Models
```

`PipelineContext<T>` is a **strongly-typed** data carrier that flows through the entire mediator pipeline. Each call to `IMediator.Send` or `IMediator.Publish` receives its own isolated instance. Behaviors and handlers read and write properties on the user-defined context type — no dictionaries, no magic strings, no boxing.

## 🎯 Purpose

`PipelineContext<T>` enables cross-cutting data to be passed between pipeline steps without modifying request or notification types. Common use cases:

- **Correlation IDs** — stamp every request with a trace identifier
- **Audit metadata** — track user identity, tenant, or session across handlers
- **Performance timers** — measure elapsed time from the outermost behavior to the innermost handler

## 📦 Type Definition

```csharp
public class PipelineContext<T> where T : class, new()
{
    public T Data { get; } = new T();
}
```

- `T` is a **user-defined class** with a parameterless constructor
- `Data` is pre-initialized — behaviors and handlers read/write its properties directly
- Each `Send`/`Publish` call creates a new instance (or accepts an externally-provided one)

## ⚙️ Defining a Context Type

```csharp
public class MyPipelineContext
{
    public string? CorrelationId { get; set; }
    public int? UserId { get; set; }
    public System.Diagnostics.Stopwatch? Timer { get; set; }
}
```

## 🚀 Usage

### Sending with context

```csharp
var ctx = new PipelineContext<MyPipelineContext>();
ctx.Data.UserId = 42;

var result = await mediator.Send<MyCommand, Result<Data>, MyPipelineContext>(command, ctx);
```

Context is opt-in: the original `Send<TRequest, TResponse>(...)` and `Publish<TNotification>(...)` overloads continue to work without a context type parameter.

### Reading context in a behavior

```csharp
public class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<Result<TResponse>> HandleAsync(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        => HandleAsync(request, _ => next(), new PipelineContext<MyPipelineContext>(), ct);

    public async Task<Result<TResponse>> HandleAsync<TContext>(
        TRequest request, RequestHandlerContextDelegate<TResponse, TContext> next,
        PipelineContext<TContext> context, CancellationToken ct)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyPipelineContext> ctx)
            ctx.Data.CorrelationId = Guid.NewGuid().ToString();
        return await next(context);
    }
}
```

### Reading context in a handler

```csharp
public class MyHandler : RequestHandler<MyCommand, Result<Data>>
{
    public MyHandler(ILogger<MyHandler> logger) : base(logger) { }

    protected override async Task<Result<Data>> HandleAsync<TContext>(
        MyCommand request, PipelineContext<TContext> context, CancellationToken ct)
        where TContext : class, new()
    {
        if (context is PipelineContext<MyPipelineContext> ctx)
            Console.WriteLine($"CorrelationId: {ctx.Data.CorrelationId}");
        return await base.HandleAsync(request, context, ct);
    }
}
```

## 🧠 Design Notes

- **No dictionaries** — the context type `T` is a plain class with properties. IntelliSense, refactoring, and compile-time safety are preserved.
- **Isolation** — the `Mediator` creates `new PipelineContext<T>()` at the start of each call (or uses the caller-provided instance). No shared/static state.
- **Backward compatible** — context type parameters on `Send`/`Publish` are additive. Existing code calling `Send<TRequest, TResponse>(...)` compiles unchanged via an internal marker type.
- **Default interface methods** — behaviors and handlers override `HandleAsync<TContext>(...)` only if they need context; the default implementation delegates to the non-context method.
