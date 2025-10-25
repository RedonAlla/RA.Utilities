```bash
Namespace: RA.Utilities.Integrations.DelegatingHandlers
```

First, a key clarification: `ProxyMessageHandler` is a static factory class, not a
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler). 

:::danger CAUTION

This is not a 
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
because proxy settings must be applied to the primary message handler.

:::

This distinction is crucial.
The primary purpose of `ProxyMessageHandler` is to centralize the logic for creating and configuring the innermost
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
with proxy settings.

Here’s why that's necessary:

1. **[`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) Pipeline Architecture**:
An [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)'s request pipeline consists of a chain of handlers.
The outermost handlers are
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)
(like your logging and auth handlers), which can inspect or modify a request and then pass it down the chain.
The very last handler in the chain is the primary message handler
(usually an [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler)),
which is responsible for actually sending the request over the network.

2. **Proxy Configuration Requirement**:
Proxy settings cannot be applied by a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler).
They must be configured directly on the properties of the primary [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler) before the pipeline is even built.

3. **A Centralized Factory**:
The `ProxyMessageHandler` acts as a factory to solve this problem.
Its static Create method takes an [`IProxySettings`](../Abstractions/IProxySettings.md) object and returns a correctly configured [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).

  * If no proxy settings are provided, it returns a default [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).
  * If proxy settings are provided, it creates a [`WebProxy`](https://learn.microsoft.com/en-us/dotnet/api/system.net.webproxy) instance,
  populates it with the address, credentials, and bypass settings,
  and assigns it to a new [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).