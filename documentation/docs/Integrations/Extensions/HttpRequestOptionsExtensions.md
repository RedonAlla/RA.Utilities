---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Integrations.Extensions
```

The `HttpRequestOptionsExtensions` class provides **extension members on [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions)** (declared inside an `extension(HttpRequestOptions)` block) for reading context that the hosting environment attaches to outgoing requests.

## `GetClientIpAddress()`

Extracts the original client's IP address from the request context.
Finding the client IP differs between hosting environments (e.g., IIS vs. self-hosted); this method encapsulates that logic into a single call, which is invaluable for logging, auditing, or rate-limiting inside a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler).

It checks the option keys used by both IIS-hosted (`MS_HttpContext`) and self-hosted (`System.ServiceModel.Channels.RemoteEndpointMessageProperty`) environments, and returns an empty string when neither is present.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpRequestOptions` | [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions) | The options collection of an [`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage). |
| **Returns** | `string` | The client's IP address, or an empty string if it cannot be found. |

#### Example

This method is typically used inside a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler), where the hosting environment populates the request options. The package's own [`LoggingHandler`](../DelegatingHandler/LoggingHandler.md) uses it to fill the `RemoteAddress` field of the log templates.

```csharp
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.Integrations.Extensions;

public class IpLoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // The hosting environment (like ASP.NET Core) populates the request options.
        var clientIp = request.Options.GetClientIpAddress();

        Console.WriteLine($"Request received from IP address: {clientIp}");

        return await base.SendAsync(request, cancellationToken);
    }
}
```

:::info[v10.2.0 rename]
This extension was previously declared in the `HttpRequestMessageExtensions` class. In v10.2.0 the class was renamed to `HttpRequestOptionsExtensions` to match the actual receiver type ([`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions)), and the method became a C# 14 extension member. Call sites such as `request.Options.GetClientIpAddress()` are unchanged; the companion `ToDictionary(...)` helper was removed — see the [migration guide](../migration-guides).
:::
