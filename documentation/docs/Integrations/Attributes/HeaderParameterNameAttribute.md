---
sidebar_position: 4
---

```bash
Namespace: RA.Utilities.Integrations.Attributes
```

The `[HeaderParameterName("...")]` attribute overrides the HTTP header name emitted for a property of a class marked with [`[HeaderParameters]`](./HeaderParametersAttribute).

By default the property name is used as the header name. This attribute is required for header names that aren't valid C# identifiers — most notably dashed names such as `x-request-id`.

## 🚀 Example

```csharp showLineNumbers
[HeaderParameters]
public partial class RequestHeaders
{
    [HeaderParameterName("x-request-id")]
    public string? XCorrelationId { get; init; }

    [HeaderParameterName("x-api-key")]
    public string? ApiKey { get; init; }
}

// Sends: x-request-id: trace-1 and x-api-key: secret
await client.GetAsync<Product>("products", headers: new RequestHeaders
{
    XCorrelationId = "trace-1",
    ApiKey = "secret"
});
```

## 📋 Rules

- Applies to a single property; at most one per property.
- Empty or whitespace-only names are ignored and the property name is used instead.
- The attribute only affects the emitted header name — member access in your code still uses the property name.
- Content headers (`Content-Type`, `Content-Length`, ...) are still rejected by `HttpRequestMessage.Headers` regardless of the name used.

## 🧠 Summary

`[HeaderParameterName]` decouples the C# property name from the HTTP header name, letting you express any header — including dashed, non-standard ones — with a clean, strongly-typed property.
