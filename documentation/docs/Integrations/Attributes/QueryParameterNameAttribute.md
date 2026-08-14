---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Integrations.Attributes
```

The `[QueryParameterName("...")]` attribute overrides the query string key emitted for a property of a class marked with [`[QueryParameters]`](./QueryParametersAttribute).

By default the property name is used as the query string key. This attribute is useful when the API expects keys that don't follow C# identifier rules (snake_case, kebab-case, dots, etc.).

## 🚀 Example

```csharp showLineNumbers
[QueryParameters]
public partial class GetProductsQuery
{
    [QueryParameterName("category_id")]
    public int? CategoryId { get; init; }

    [QueryParameterName("page[size]")]
    public int PageSize { get; init; }
}

// GET /products?category_id=5&page%5Bsize%5D=20
await client.GetAsync<Product>("products", new GetProductsQuery { CategoryId = 5, PageSize = 20 });
```

The overridden key is URL-encoded like any other key or value by [`QueryUtilities`](../Utilities/QueryUtilities).

## 📋 Rules

- Applies to a single property; at most one per property.
- Empty or whitespace-only names are ignored and the property name is used instead.
- The attribute only affects the emitted key — member access in your code still uses the property name.

## 🧠 Summary

`[QueryParameterName]` decouples the C# property name from the wire format, keeping your models idiomatic while matching any external API's naming convention exactly.
