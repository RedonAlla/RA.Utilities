---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Attributes
```

The `[QueryParameters]` attribute marks a `partial` class or record as a strongly-typed container of query string parameters.

At compile time, the package's source generator emits a partial declaration that implements [`IQueryStringRequest`](../Abstractions/IQueryStringRequest.md) for the class, mapping each public instance property to a query string key-value pair.

## ⚙️ How It Works

Given:

```csharp showLineNumbers
[QueryParameters]
public partial class GetProductsQuery
{
    public int? CategoryId { get; init; }
    public string? Search { get; init; }
    public int[] Ids { get; init; } = [];
}
```

the generator produces (conceptually):

```csharp showLineNumbers
public partial class GetProductsQuery : IQueryStringRequest
{
    public QueryParams QueryStringValues()
    {
        var values = new QueryParams();

        // CategoryId → only when not null
        // Search    → only when not null
        // Ids       → one pair per element: ?Ids=1&Ids=2

        return values;
    }

    public string ToQueryString(string action) => ...;
}
```

Callers can then pass the object directly to [`BaseHttpClient`](../BaseHttpClient):

```csharp showLineNumbers
await client.GetAsync<Product>("products", new GetProductsQuery { CategoryId = 5 });
// GET /products?CategoryId=5
```

## 📋 Rules

- The class **must be `partial`** — and so must all of its containing types (the generator reports error `RPIG001` otherwise).
- Only public instance properties with a getter are mapped; static properties and indexers are ignored.
- **`null` values are skipped.** Non-nullable value types (e.g. `int`, `bool`) have no null state and are always emitted — declare them as `int?`/`bool?` to make them optional.
- `bool` values are formatted as `true`/`false`; `DateTime`/`DateTimeOffset` use the round-trip `"o"` format; other scalars use invariant culture formatting.
- Collections (`IEnumerable<T>`, arrays) produce repeated keys; dictionaries produce one entry per key-value pair, using the entry's key.
- Use [`[QueryParameterName]`](./QueryParameterNameAttribute) on a property to override the emitted key.
- Classes that already implement `IQueryStringRequest` (or declare a `QueryStringValues`/`ToQueryString` member) are skipped to avoid duplicate members.

## 🧠 Summary

`[QueryParameters]` turns a plain DTO into a compile-time-safe, self-mapping query string model — no hand-written mapping code, no runtime reflection, and full IntelliSense support at the call site.
