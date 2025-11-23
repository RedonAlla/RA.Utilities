---
title: RequestIdEnricher
---

The `RequestIdEnricher` class is a custom **Serilog enricher** designed to automatically add crucial correlation
IDs to every log event generated during an HTTP request.

Its primary purpose is to make your logs more traceable and debuggable, especially in a microservices or distributed environment. When a single user action results in calls across multiple services, having a consistent ID allows you to filter and find all related log entries across all systems.

## ⚙️ Here's how it works:

### 1. Adds `XRequestId`:
It first checks the incoming HTTP request for a header named `x-request-id`.
If found, it adds this value to the log event as a property called `XRequestId`.
This is a common pattern for propagating a unique correlation ID that originates from the initial client or an upstream service.

### 2. Adds `TraceId`:
It then establishes a `TraceId` for the log event by checking the following sources in order:

* It prefers the `HttpContext.TraceIdentifier`, which is ASP.NET Core's unique identifier for a single request on a single server.
* If that is not available (e.g., in a background process not initiated by a web request), it falls back to `Activity.Current`,
which is part of .NET's modern distributed tracing system.


By adding these properties to every log message, you can easily group all logs related to a single operation, which is invaluable for debugging complex issues.