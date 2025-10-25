```bash
Namespace: RA.Utilities.Integrations.DelegatingHandlers
```

The `RequestResponseLoggingHandler` is a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
that acts as middleware for your [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) instances.
Its primary purpose is to **intercept and log the details of outgoing HTTP requests and their corresponding responses**.
This provides a comprehensive audit trail and invaluable diagnostic information for any communication between your services and external APIs.

It achieves this by performing the following steps within the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) pipeline:

1. **Intercepts the Call**:
When an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) sends a request, this handler receives it before it goes out over the network.
2. **Executes the Request**: It passes the request down the handler chain (using `base.SendAsync`) and waits for the
[`HttpResponseMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage) to return.
3. **Logs the Request**:
After the call completes, it constructs a detailed `HttpRequestLogTemplate` object containing:
  * The request method, URI, path, and query string.
  * Request headers.
  * The request body (which it intelligently tries to parse as JSON for better structured logging).
  * The `TraceIdentifier` from the original incoming request, linking this outgoing call to the parent operation.
4. **Logs the Response**: It then constructs a `HttpResponseLogTemplate` object with:
  * The HTTP status code.
  * Response headers.
  * The response body.
5. **Structured Logging**:
It uses [`ILogger`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger) to write these objects as structured logs.
This is far more powerful than plain text logging, as it allows you to easily search, filter, and analyze your HTTP traffic in logging tools like Seq, Datadog, or the ELK stack.

The key benefit is deep visibility. When debugging an issue, you can see the exact payload your application sent to another service and the exact response it received, which is critical for troubleshooting integration problems.

## ⚙️ How It's Used
This handler is typically added to an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) during its registration in `Program.cs` using the provided extension method:

```csharp
services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(...)
    .WithHttpLoggingHandler(); // <-- This adds the handler to the pipeline
```
