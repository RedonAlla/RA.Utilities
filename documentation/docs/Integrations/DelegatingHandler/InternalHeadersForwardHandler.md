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
The handler is smart enough to use the incoming `x-request-id` if it exists, and falls back to the `HttpContext.TraceIdentifier` if it doesn't.

## ⚙️ How It Works
### 1. Capturing Context:
When the handler is created (once per request scope), its constructor uses
[`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor?view=aspnetcore-9.0)
to access the current incoming HTTP request.
It reads the Authorization and x-request-id headers and stores them in private fields.

### 2. Injecting Headers:
When the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) sends a request, the handler's `SendAsync` method is invoked.
It adds the captured `Authorization` token and the `RequestId` to the headers of the outgoing [`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage).

### 3. Passing the Request:
After modifying the headers, it passes the request to the next handler in the pipeline.

This design elegantly solves the problem of maintaining security and traceability in a distributed system, abstracting the logic away from the application code. A developer simply needs to add this handler to their [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) configuration for an internal service, and the header propagation happens automatically.