---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Logging.Shared.Models.HttpLog
```

The `HttpRequestLogTemplate` class is a specialized model used for creating structured log entries for an HTTP request.
It inherits from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) to include common properties and adds details specific to the request itself.

## Properties
These are the properties defined on the `HttpRequestLogTemplate` class, including those inherited from `BaseHttpLogTemplate`.


| Property | Type | Description | Inherited |
| -------- | ---- | ----------- | --------- |
| **RequestId** | `string?` | The value of the `x-request-id` header from the HTTP request. Provides end-to-end correlation across services. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **TraceIdentifier** | `string?` | The identifier from `HttpContext.TraceIdentifier`, used for end-to-end tracing within the ASP.NET Core pipeline. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **Path** | `string?` | The URI path of the request (e.g., `/api/users/123`). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RequestedOn** | `DateTime` | The date and time the request was made, in UTC. Defaults to `DateTime.UtcNow` when the instance is created. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RemoteAddress** | `string?` | The client's IP address (for incoming requests) or the target server's host name/IP (for outgoing requests). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **Scheme** | `string?` | The URI scheme used by the request message (e.g., "http" or "https"). | - |
| **Method** | `string?` | The HTTP method used by the request (e.g., "GET", "POST"). | - |
| **Host** | `string?` | The host name from the request, typically from the `Host` header. | - |
| **QueryString** | `string?` | The query string portion of the request URI. | - |
| **RequestHeaders** | `IDictionary<string, string>?` | A dictionary containing the HTTP headers sent with the request. | - |
| **RequestBody** | `object?` | The body of the HTTP request, if any. | - |
