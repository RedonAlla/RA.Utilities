---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Logging.Shared.Models.HttpLog
```

The `BaseHttpLogTemplate` class serves as the **foundational base class for creating structured log entries for HTTP interactions**.

Its primary purpose is to define a standardized set of common properties that are relevant to both an HTTP request and its corresponding response.
By having more specific log models like `HttpRequestLogTemplate` and `HttpResponseLogTemplate` inherit from it, you ensure that all your HTTP logs have a consistent, predictable core structure.


## Properties
| Property |	Type |	Description |
| -------- |	---- |	----------- |
| **RequestId** |	`string?` |	A unique identifier generated for the request, used for correlation across logs. |
| **TraceIdentifier** |	`string?` |	The identifier from `HttpContext.TraceIdentifier`, used for end-to-end tracing within the ASP.NET Core pipeline. |
| **Path** |	`string?` |	The URI path of the request (e.g., `/api/users/123`). |
| **RequestedOn** |	`DateTime` |	The UTC timestamp indicating when the log object was created. |
| **RemoteAddress** |	`string?` |	The client's IP address (for incoming requests) or the target server's host name/IP (for outgoing requests). |


## Here's a breakdown of its key roles:

### 1. Standardizes Core Log Properties:
It establishes a contract for essential logging information, including:

  * `RequestId` and `TraceIdentifier` for correlating and tracing requests across services.
  * `Path` for identifying the resource being accessed.
  * `RequestedOn` for timestamping the event.
  * `RemoteAddress` for identifying the client IP or target host.

### 2. Enables Code Reusability:
It prevents duplication of these common properties in the `HttpRequestLogTemplate` and `HttpResponseLogTemplate` classes,
adhering to the Don't Repeat Yourself (DRY) principle.

### 3. Facilitates Structured Logging:
The `ToString()` method, which serializes the object to JSON, is a key feature.
It allows logging providers like Serilog to easily capture the entire object as a structured log entry,
making logs machine-readable and much easier to query and analyze in log management systems.


## 🧠 Summary
In short, `BaseHttpLogTemplate` is the blueprint that guarantees consistency and provides the core data for all HTTP logging within your `RA.Utilities` ecosystem.