---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Logging.Shared.Models.HttpLog
```

The `HttpResponseLogTemplate` class is a specialized model used for creating structured log entries for an HTTP response.
It inherits from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) to include common properties and adds details specific to the response itself.

## Properties
| Property |	Type |	Description | Inherited |
| -------- |	---- |	----------- | --------- |
| **RequestId** |	`string?` |	A unique identifier generated for the request, used for correlation across logs. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **TraceIdentifier** |	`string?` |	The identifier from `HttpContext.TraceIdentifier`, used for end-to-end tracing within the ASP.NET Core pipeline. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **Path** |	`string?` |	The URI path of the request (e.g., `/api/users/123`). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RequestedOn** |	`DateTime` |	The UTC timestamp indicating when the log object was created. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RemoteAddress** |	`string?` |	The client's IP address (for incoming requests) or the target server's host name/IP (for outgoing requests). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RequestId** |	`string?` |	A unique identifier generated for the request, used for correlation. | - |
| **TraceIdentifier** |	`string?` |	The identifier from `HttpContext.TraceIdentifier` for end-to-end tracing. | - |
| **Path** |	`string?` |	The URI path of the request (e.g., `/api/users/123`). | - |
| **RequestedOn** |	`DateTime` |	The UTC timestamp when the log object was created. | - |
| **RemoteAddress** |	`string?` |	The client's IP address or the target server's host name. | - |