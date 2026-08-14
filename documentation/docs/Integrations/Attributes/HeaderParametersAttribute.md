---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations.Attributes
```

The `[HeaderParameters]` attribute marks a `partial` class or record as a strongly-typed container of HTTP header values.

At compile time, the package's source generator emits a partial declaration that implements [`IHeaderRequest`](../Abstractions/IHeaderRequest.md) for the class, mapping each public instance property to a header key-value pair.

## ⚙️ How It Works

Given:

```csharp showLineNumbers
[HeaderParameters]
public partial class RequestHeaders
{
    public string? XCorrelationId { get; init; }
    public string? AcceptLanguage { get; init; }
}
```

the generator produces (conceptually):

```csharp showLineNumbers
public partial class RequestHeaders : IHeaderRequest
{
    public Dictionary<string, string> ToHeaders()
    {
        var values = new Dictionary<string, string>();

        // XCorrelationId → only when not null
        // AcceptLanguage → only when not null

        return values;
    }
}
```

Callers can then pass the object directly to [`BaseHttpClient`](../BaseHttpClient):

```csharp showLineNumbers
await client.GetAsync<Product>("products", headers: new RequestHeaders { XCorrelationId = "trace-1" });
```

## 📋 Rules

- The class **must be `partial`** — and so must all of its containing types (the generator reports error `RPIG001` otherwise).
- Only public instance properties with a getter are mapped; static properties and indexers are ignored.
- **`null` values are skipped.** Non-nullable value types (e.g. `int`, `bool`) have no null state and are always emitted — declare them as `int?`/`bool?` to make them optional.
- The default header name is the property name. Use [`[HeaderParameterName]`](./HeaderParameterNameAttribute) on a property to override it (e.g. for dashed names like `x-request-id`).
- Dictionaries produce one header per entry; duplicate names are resolved by last-write-wins (no exception).
- Content headers (`Content-Type`, `Content-Length`, ...) cannot be set this way — `HttpRequestMessage.Headers` rejects them. `Content-Type` is managed automatically for JSON bodies.
- Classes that already implement `IHeaderRequest` (or declare a `ToHeaders` member) are skipped to avoid duplicate members.

## 🧠 Summary

`[HeaderParameters]` turns a plain DTO into a compile-time-safe, self-mapping header model — no dictionaries of magic strings, no runtime reflection, and full IntelliSense support at the call site.
