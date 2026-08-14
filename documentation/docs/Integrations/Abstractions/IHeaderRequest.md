---
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

The `IHeaderRequest` interface provides a standardized contract for request models that need to supply HTTP header values.
Its main goal is a clean, reusable, and type-safe pattern for defining headers on HTTP requests — the header counterpart of [`IQueryStringRequest`](./IQueryStringRequest.md).

## ⚙️ How It Works

The interface has a single member:

| Method | Return | Description |
| ------ | ------ | ----------- |
| **ToHeaders()** | `Dictionary<string, string>` | Returns the header names and values for the request. |

You rarely implement it by hand: mark a `partial` class with [`[HeaderParameters]`](../Attributes/HeaderParametersAttribute.md) and the source generator implements the interface for you, mapping each public property to a header key-value pair.

## 🚀 Example Usage

### 1. Define the Request Model

```csharp showLineNumbers
using RA.Utilities.Integrations.Attributes;

[HeaderParameters]
public partial class RequestHeaders
{
    [HeaderParameterName("x-request-id")]
    public string? XCorrelationId { get; init; }
}
```

### 2. Use It with the Client

```csharp showLineNumbers
await client.GetAsync<Product>(
    "products/1",
    headers: new RequestHeaders { XCorrelationId = "trace-1" });
// Sends: x-request-id: trace-1
```

## 📋 Rules

- Only public instance properties with a getter are mapped; **null values are skipped**.
- The default header name is the property name; override it with [`[HeaderParameterName]`](../Attributes/HeaderParameterNameAttribute.md).
- Content headers (`Content-Type`, `Content-Length`, ...) cannot be set through this contract — `HttpRequestMessage.Headers` rejects them. `Content-Type` is managed automatically for JSON request bodies.

## 🧠 Summary

`IHeaderRequest` gives header models a common contract so that `BaseHttpClient` (and your own code) can consume any header class without knowing its concrete type — while the generator keeps the definition of those classes effortless and compile-time safe.
