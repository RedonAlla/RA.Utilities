---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Utilities
```

The `QueryUtilities` class is the single source of truth for turning query parameters into URL-encoded query strings.
It is used by [`BaseHttpClient`](../BaseHttpClient), the default implementation of [`IQueryStringRequest.ToQueryString`](../Abstractions/IQueryStringRequest.md), and the code generated for [`[QueryParameters]`](../Attributes/QueryParametersAttribute.md) classes.

## ⚙️ Methods

| Method | Description |
|---|---|
| `ToQueryString(QueryParams? request)` | URL-encodes a collection of key-value pairs and returns them prefixed with `?`, or an empty string when the collection is null, empty, or has no non-empty values. |
| `ToQueryString(string action, IQueryStringRequest? request)` | Appends the query string of a query request model to an action/endpoint path; returns just the action when the request is null. |

## 🚀 Example

```csharp showLineNumbers
using RA.Utilities.Integrations.Models;
using RA.Utilities.Integrations.Utilities;

QueryParams parameters =
[
    new("search term", "a b"),
    new("path", "a/b&c"),
    new("empty", string.Empty)   // skipped
];

string queryString = QueryUtilities.ToQueryString(parameters);
// "?search%20term=a%20b&path=a%2Fb%26c"
```

```csharp showLineNumbers
// With a query request model:
string uri = QueryUtilities.ToQueryString("products", query); // "products?CategoryId=5"
```

## 📋 Behavior

- Keys and values are escaped with `Uri.EscapeDataString` (the stricter, modern escaping for query components).
- Parameters with `null` or empty values are skipped.
- Parameter order follows the insertion order of the collection.
- The `DictionaryExtensions.ToQueryString(this QueryParams)` extension was removed in v10.1.0 — this class is now the single entry point for query string construction.

## 🧠 Summary

`QueryUtilities` centralizes query string construction so that every request — hand-built, generated, or made through `BaseHttpClient` — is encoded identically.
