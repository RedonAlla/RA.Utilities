---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Models
```

The `QueryParams` class is a specialized collection designed to make it easy to build URL query strings.
It represents an ordered list of string key-value pairs — exactly the shape of query parameters — and is the return type of [`IQueryStringRequest.QueryStringValues()`](../Abstractions/IQueryStringRequest.md),
which means every query string built by [`BaseHttpClient`](../BaseHttpClient) and the generated [`[QueryParameters]`](../Attributes/QueryParametersAttribute.md) classes flows through this type.

## 🎯 Purpose

At its core, `QueryParams` is a **list of key-value pairs** with two convenience members:

- a simplified `Add(key, value)` overload, and
- a `ToString()` override that URL-encodes the collection into a valid query string.

It gives the integration layer a clear, intent-specific type instead of raw `Dictionary<string, string>` or hand-joined strings, and it supports **duplicate keys** — a valid and common pattern in URL query strings (e.g. `?filter=A&filter=B`).

## ⚙️ How It Works

### 1. Inheritance from `List<KeyValuePair<string, string>>`

```csharp showLineNumbers
public class QueryParams : List<KeyValuePair<string, string>>
{
}
```

Because it inherits from `List`, it supports:

| Feature | Example |
|---|---|
| Collection initializer | `new QueryParams { { "page", "1" }, { "size", "20" } }` |
| Duplicate keys | `?filter=A&filter=B` |
| Indexed access | `queryParams[0]` |
| Standard list operations | `Count`, `Clear()`, `Remove(...)`, LINQ |

### 2. Convenience `Add` method

```csharp showLineNumbers
public void Add(string key, string value)
{
    Add(new KeyValuePair<string, string>(key, value));
}
```

Adds a parameter with a simple `Add("name", "gemini")` call instead of the verbose `Add(new KeyValuePair<string, string>("name", "gemini"))` required by the base `List` class.

```csharp showLineNumbers
var queryParams = new QueryParams();
queryParams.Add("page", "1");
queryParams.Add("user name", "Gemini Assist"); // keys with spaces are allowed — encoded later
```

### 3. `ToString()` — URL-encoded output

```csharp showLineNumbers
public override string ToString()
{
    if (Count == 0)
    {
        return string.Empty;
    }

    IEnumerable<string> segments = this.Select(kvp =>
        $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");

    return string.Join("&", segments);
}
```

`ToString()` returns the encoded `key=value` pairs joined with `&` — **without** a leading `?` — or an empty string when the collection is empty.

```csharp showLineNumbers
var queryParams = new QueryParams
{
    { "page", "1" },
    { "user name", "Gemini Assist" } // Key with a space
};

// "page=1&user+name=Gemini+Assist"
string urlQuery = queryParams.ToString();
```

#### What is `WebUtility.UrlEncode`?

`WebUtility.UrlEncode` is a standard .NET method that converts a string into a URL-safe format.
URLs have a restricted set of allowed characters — spaces, ampersands (`&`), question marks (`?`), and equal signs (`=`) are "reserved" because they have structural meaning within a URL.
`UrlEncode` replaces unsafe characters with a percent sign (`%`) followed by their two-digit hexadecimal representation:

| Character | Encoded |
|---|---|
| Space (` `) | `%20` (or `+` — see note below) |
| Ampersand (`&`) | `%26` |
| Plus (`+`) | `%2B` |
| Hash (`#`) | `%23` |

This guarantees that the keys and values you intend to send are the same keys and values the server receives, regardless of what special characters they contain.

> **Note on encodings**: `WebUtility.UrlEncode` historically leaves spaces as `+` in some contexts. When requests are actually sent, the final query string is built by [`QueryUtilities.ToQueryString`](../Utilities/QueryUtilities.md), which uses the stricter `Uri.EscapeDataString` (spaces become `%20`) and skips empty values. `ToString()` remains a convenience for display and debugging — the two paths agree on everything except these edge characters.

## 🧩 How It Fits into the Ecosystem

```
Your [QueryParameters] class ──generator──► QueryStringValues() ──► QueryParams
                                                                        │
                                              QueryUtilities.ToQueryString (Uri.EscapeDataString)
                                                                        │
                                              BaseHttpClient ──► GET /products?page=1&size=20
```

- The generated code and hand-written `IQueryStringRequest` implementations return `QueryParams`.
- [`QueryUtilities.ToQueryString(QueryParams?)`](../Utilities/QueryUtilities.md) consumes it, URL-encodes keys and values, skips `null`/empty values, and prefixes the result with `?`.
- The `DictionaryExtensions.ToQueryString(this QueryParams?)` extension delegates to `QueryUtilities` and remains available for backward compatibility.

## 🧠 Summary

In short, `QueryParams` is a small helper class that improves developer experience by providing a clear, intent-specific type for handling URL parameters, a simpler way to add them, and reliable URL encoding — while the actual request pipeline layers stricter encoding and empty-value skipping on top via `QueryUtilities`.
