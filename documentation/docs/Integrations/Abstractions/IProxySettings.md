---
sidebar_position: 3
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

The `IProxySettings` interface defines a standardized contract for configuration classes that need to provide settings for an HTTP proxy server.
Its primary purpose is to decouple the HTTP client configuration logic from any specific implementation of settings,
allowing for flexible and reusable proxy configuration.

This serves a few key goals:

1. **Standardization**:
It ensures that any integration requiring a proxy has a consistent way of exposing its configuration (Address, Credentials, etc.).
This creates a predictable structure for developers.

2. **Abstraction**:
It allows the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) setup logic,
specifically the `WithProxyFromSettings` extension method, to operate on the `IProxySettings` contract rather than a concrete class.
This means you can have different settings classes for different integrations, and as long as they can provide an object that implements `IProxySettings`, the proxy configuration will work seamlessly.

3. **Configuration Binding**:
It provides a clear target for binding proxy-related settings from a configuration source like `appsettings.json`.
A concrete class like `ProxySettings` implements this interface and can be nested within a larger integration-specific settings object.

## ⚙️ How It Works in Your Project
The `IProxySettings` interface is a key part of a chain of components that work together to configure the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) pipeline:

#### 1. The Contract (`IProxySettings.cs`):
Defines the essential properties for a proxy: `Address`, `Username`, `Password`, and `BypassProxyOnLocal`.

```csharp showLineNumbers
public interface IProxySettings
{
    string? Address { get; set; }
    string? Username { get; set; }
    string? Password { get; set; }
    bool BypassProxyOnLocal { get; set; }
}
```

#### 2. The Factory (`ProxyMessageHandler.cs`):
The `ProxyMessageHandler.Create` static method takes an `IProxySettings` object and is responsible for creating and configuring an
[`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).
It translates the properties from the interface into a concrete `WebProxy` instance, which is then assigned to the handler.

The Extension Method (DependencyInjectionExtensions.cs): The WithProxyFromSettings extension method connects the DI container to the ProxyMessageHandler. It configures the HttpClient's primary message handler by:

#### 3. Resolving the main integration settings (`TSettings`).
Using a selector function to extract the `IProxySettings` from `TSettings`.
Calling `ProxyMessageHandler.Create()` to build the final, proxy-aware [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).

This design is powerful because the proxy configuration is applied to the innermost handler in the
[`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler)
pipeline, which is a requirement for setting a proxy.

## Properties

This interface provides a standardized way to define proxy server configurations for [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler) instances.

| Property |	Type |	Description |
| -------- | ----- | ------------ |
| **Address** |	`string?` |	The URI of the proxy server. Must be a valid URL format (e.g., `"http://proxy.example.com:8888"`). |
| **Username** |	`string?` |	`string?	The username used for proxy authentication, if required. |
| **Password** |	`string?` |	`string?	The password used for proxy authentication, if required. |
| **BypassProxyOnLocal** |	`string?` |	bool	A flag indicating whether the proxy should be bypassed for local (intranet) addresses. |

## 🧠 Summary
In short, `IProxySettings` is an abstraction that standardizes how proxy configurations are defined.
It enables a clean, reusable, and type-safe mechanism for applying proxy settings to any
[`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) registered through your `RA.Utilities.Integrations` framework, promoting loose coupling and clear separation of concerns.