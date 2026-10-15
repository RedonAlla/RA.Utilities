```bash
Namespace: RA.Utilities.Integrations.DelegatingHandlers
```

The `InternalHeadersForwardHandler` is a
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
designed specifically for **internal service-to-service communication** within a microservices architecture.
Its primary purpose is to automatically propagate critical context from an incoming HTTP request to an outgoing HTTP request made by the service.

It achieves this by intercepting outgoing [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) calls and forwarding two key pieces of information:

1. **Authentication Context (`Authorization` header)**:
It copies the `Authorization` header (typically containing a JWT bearer token) from the original request that the service received.
This allows the downstream service to identify and authorize the original user without requiring the calling service to manage or re-issue tokens.
It effectively preserves the user's identity across service boundaries.

2. **Traceability Context (`x-request-id` header)**:
It forwards a request identifier, which is crucial for distributed tracing.
By ensuring every service call in a chain shares the same ID, you can easily correlate logs and traces across multiple systems to debug issues or monitor the flow of a single user operation.
The handler uses the incoming `x-request-id` value and falls back to the `HttpContext.TraceIdentifier` when the `x-request-id` value is not available.

## ⚙️ How It Works
### 1. Resolving the Current Context:
The handler is constructed with an
[`IHttpContextAccessor`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-9.0),
which it stores for later use.

### 2. Reading Headers Per Request:
When the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) sends a request, the handler's `SendAsync` method is invoked.
It reads the current `Authorization` header, `x-request-id` header, and trace identifier **at that moment** from `IHttpContextAccessor.HttpContext` —
so every outgoing request gets the headers of the HTTP request that triggered it, even when the underlying handler instance is reused across requests.

### 3. Injecting Headers:
It adds the read token and request ID to the headers of the outgoing [`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage) and passes the request to the next handler in the pipeline.
If the context or a header is not available, the corresponding value is an empty string, so no exception is thrown.

This design elegantly solves the problem of maintaining security and traceability in a distributed system, abstracting the logic away from the application code. A developer simply needs to add this handler to their [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) configuration for an internal service, and the header propagation happens automatically.

:::info[v10.2.0 behavior change]
Before v10.2.0 the handler captured the `Authorization` and `x-request-id` headers **once, in the constructor**, and reused those captured values for every request the handler instance served. Since `HttpMessageHandler` instances are pooled by `IHttpClientFactory`, the captured values could go stale. Since v10.2.0 the values are read on every `SendAsync` — no code changes are required, but manually constructed handlers now need the same `IHttpContextAccessor` semantics.
:::
