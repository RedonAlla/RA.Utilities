---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations.Extensions
```

The `HttpRequestMessageExtensions` class is a static utility class that provides a set of convenient,
reusable extension methods for working with
[`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage)
and its related components like [`HttpRequestHeaders`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httprequestheaders) and [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions).

Its primary purpose is to **simplify common, and sometimes complex, operations** that you might perform on HTTP requests,
particularly within the context of [`DelegatingHandlers`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) or other parts of the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) pipeline.

## Here's a breakdown of what each method does:

### 1. GetClientIpAddress(this [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions) httpRequestOptions):

This method extracts the original client's IP address from the request context.
Finding the client IP can be complex as it differs between hosting environments (e.g., IIS vs. self-hosted).
This method encapsulates that logic into a single, reliable call, which is invaluable for logging, auditing, or rate-limiting.

#### Input parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpRequestOptions` | [`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions) | The options collection from an [`HttpRequestMessage`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage)`. This is the extension method's target. |

#### Output parameters
| Parameter | Type | Description |
| :--- | :--- | :--- |
| **Returns** | `string` | The client's IP address, or an empty string if it cannot be found. |

#### Example

This method is typically used inside a [`DelegatingHandlers`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler), where the hosting environment populates the
[`HttpRequestOptions`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestoptions).

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
        // This method checks for keys used by both self-hosted ('RemoteEndpointMessageProperty')
        // and IIS-hosted ('MS_HttpContext') environments.
        var clientIp = request.GetOptions().GetClientIpAddress();

        Console.WriteLine($"Request received from IP address: {clientIp}");

        return await base.SendAsync(request, cancellationToken);
    }
}
```

### 3. ToDictionary(this [`HttpHeaders`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpheaders) headers):

This method converts a collection of HTTP headers into a simple `Dictionary<string, string>`.
This provides an easy way to serialize headers for logging, inspection, or transmission.
It correctly handles headers with multiple values by joining them with a comma.

#### Input parameters
| Parameter | Type | Description |
| :--- | :--- | :--- |
| `headers` | [`HttpHeaders`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpheaders) | The [`HttpHeaders`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpheaders) collection to convert. This is the extension method's target. |

#### Output parameters
| Parameter | Type | Description |
| :--- | :--- | :--- |
| **Returns** | `Dictionary<string, string>?` | A dictionary representation of the headers, or `null` if the input is `null`. |

#### Example

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using RA.Utilities.Integrations.Extensions;

var request = new HttpRequestMessage();
request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
request.Headers.Add("User-Agent", "My-App/1.0");
request.Headers.Add("X-Forwarded-For", "203.0.113.195");
request.Headers.Add("X-Forwarded-For", "70.41.3.18"); // Header with multiple values

var headersDictionary = request.Headers.ToDictionary();

// The dictionary will contain:
// {
//   "Accept": "application/json",
//   "User-Agent": "My-App/1.0",
//   "X-Forwarded-For": "203.0.113.195,70.41.3.18"
// }
foreach (var header in headersDictionary)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

## 🧠 Summary
In short, this class is a toolkit of helpers that reduces boilerplate code, encapsulates complex logic, and makes working with HTTP requests more fluent and less error-prone.