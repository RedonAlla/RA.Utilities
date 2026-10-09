---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Logging.Shared.Models.HttpLog
```

The `HttpResponseLogTemplate` class is a specialized model used for creating structured log entries for an HTTP response.
It inherits from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) to include common properties and adds details specific to the response itself.

## Properties
These are the properties defined on the `HttpResponseLogTemplate` class, including those inherited from `BaseHttpLogTemplate`.


| Property | Type | Description | Inherited |
| -------- | ---- | ----------- | --------- |
| **Path** | `string?` | The URI path of the request (e.g., `/api/users/123`). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RequestedOn** | `DateTime` | The date and time the request was made, in UTC. Defaults to `DateTime.UtcNow` when the instance is created. | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **RemoteAddress** | `string?` | The client's IP address (for incoming requests) or the target server's host name/IP (for outgoing requests). | Inherited from [`BaseHttpLogTemplate`](BaseHttpLogTemplate.md) |
| **StatusCode** | `int?` | The HTTP status code of the response (e.g., 200, 404, 500). | - |
| **ResponseHeaders** | `IDictionary<string, string>?` | A dictionary containing the HTTP headers from the response. | - |
| **ResponseBody** | `object?` | The body of the HTTP response, if any. | - |
| **Duration** | `string?` | The total time taken to process the request and generate the response, as a pre-formatted string (e.g., `"15.42 ms"`). Built-in consumers format it as `{milliseconds} ms`. | - |
