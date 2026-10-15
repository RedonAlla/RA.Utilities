```bash
Namespace: RA.Utilities.Integrations.DelegatingHandlers
```

The `LoggingHandler` is a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
that acts as middleware for your [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) instances.
Its primary purpose is to **intercept and log the details of outgoing HTTP requests and their corresponding responses**.
This provides a comprehensive audit trail and invaluable diagnostic information for any communication between your services and external APIs.

It achieves this by performing the following steps within the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) pipeline:

1. **Executes the Request**: It passes the request down the handler chain (using `base.SendAsync`) and waits for the
[`HttpResponseMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage) to return.
2. **Logs the Request**:
After the call completes, it constructs a detailed `HttpRequestLogTemplate` object containing:
  * The request scheme, host, method, path, and query string.
  * Request headers.
  * The request body (which it intelligently tries to parse as JSON for better structured logging).
3. **Logs the Response**: It then constructs an `HttpResponseLogTemplate` object with:
  * The HTTP status code.
  * Response headers.
  * The response body.
4. **Structured Logging**:
It uses [`ILogger`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger) to write these objects as structured logs.
This is far more powerful than plain text logging, as it allows you to easily search, filter, and analyze your HTTP traffic in logging tools like Seq, Datadog, or the ELK stack.

The key benefit is deep visibility. When debugging an issue, you can see the exact payload your application sent to another service and the exact response it received, which is critical for troubleshooting integration problems.

## ⚙️ How It's Used
This handler is typically added to an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) during its registration in `Program.cs`:

```csharp
services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(...)
    .AddLoggingHandler(); // <-- This adds the handler to the pipeline
```

## Log Levels & Correlation

Both the request and the response logs are written at `LogLevel.Information`, and logging is skipped entirely when `Information` is not enabled. After the response returns, the handler starts a stopwatch and reads the request/response bodies for logging — the logged `Duration` therefore measures the time spent capturing the log payload, not the network call. If that capture takes longer than the handler's warning threshold (35 seconds), an additional `Warning` entry is written: `HttpClient call to {Method} {Url} took too long ({ElapsedMilliseconds} ms)`. When a body cannot be parsed as JSON, it is logged as a plain string with a `Warning` explaining the deserialization failure.

Request correlation (`X-Request-Id`) is carried by the ambient logging scope — for example the one created by `LoggingMiddleware` or the Serilog `RequestIdEnricher` from `RA.Utilities.Logging.Core` — not by properties on the log templates.

:::info[v10.2.0 rename]
This handler was previously named `RequestResponseLoggingHandler`. In v10.2.0 it was renamed to `LoggingHandler`; its constructor still takes only an `ILogger<LoggingHandler>`, and [`AddLoggingHandler()`](../Extensions/HttpClientBuilderExtensions.md#addlogginghandler) attaches it — only update your code if you construct or register the handler manually. See the [migration guide](../migration-guides).
:::
