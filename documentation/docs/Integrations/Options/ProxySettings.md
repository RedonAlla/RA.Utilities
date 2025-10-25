```bash
Namespace: RA.Utilities.Integrations.Options
```

The `ProxySettings` class serves as a **strongly-typed configuration** model specifically
designed to hold all the necessary settings for an HTTP proxy server.

Its primary purpose is to act as a concrete implementation of the [`IProxySettings`](../Abstractions/IProxySettings.md) interface,
making it the perfect target for binding a configuration section from a file like `appsettings.json`.

## Here’s a breakdown of its role in the `RA.Utilities` framework:

### 1. Configuration Binding:
This class is a simple POCO (Plain Old C# Object).
Its properties (`Address`, `Username`, `Password`, `BypassProxyOnLocal`) directly map to the values you would define in a configuration source.
This allows the .NET configuration system to automatically populate an instance of this class with your proxy details.

### 2. Decoupling and Abstraction:
By implementing the [`IProxySettings`](../Abstractions/IProxySettings.md) interface, it decouples the components that use the proxy settings
(like the [`ProxyMessageHandler`](../DelegatingHandler/ProxyMessageHandler.md)) from the concrete class itself.
The `WithProxyFromSettings` extension method can work with any settings object as long as it can provide an [`IProxySettings`](../Abstractions/IProxySettings.md) implementation, making the system flexible and extensible.

### 3. Centralization of Logic:
It provides a single, clear, and dedicated place for all proxy-related configuration.
This avoids scattering proxy settings across different parts of your application and makes it easy to manage and understand how the proxy is configured.

## Properties

| Property	| Type	| Description |
| --------- | ----- | ----------- |
| **Address**	| `string?` |	The URI of the proxy server (e.g., `"http://proxy.example.com:8888"`). |
| **Username**	| `string?` |	The username for proxy authentication, if required. |
| **Password**	| `string?` |	The password for proxy authentication, if required. |
| **BypassProxyOnLocal**	| `bool` |	A flag indicating whether the proxy should be bypassed for local (intranet) addresses. |


In short, `ProxySettings` is the data container that bridges the gap between your configuration files (e.g., `appsettings.json`) and the code responsible for creating a proxy-aware [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler).
It makes the process of configuring a proxy clean, type-safe, and consistent.

## How It's Used
A typical usage pattern would look like this:

### 1. `appsettings.json`:
You would define the proxy details in your configuration file.

```json showLineNumbers
"MyApi": {
  "BaseUrl": "https://api.example.com",
  "UseProxy": true,
  "Proxy": {
    "Address": "http://proxy.server:8080",
    "Username": "proxy-user",
    "Password": "proxy-password",
    "BypassProxyOnLocal": true
  }
}
```

### 2. Concrete Settings Class:
You would have a settings class for your API that includes the `ProxySettings`.

```csharp showLineNumbers
public class MyApiSettings : BaseApiSettings<MyApiActions>
{
    public ProxySettings Proxy { get; set; }
}
```

### 3. Dependency Injection:
In `Program.cs`, you would use the `WithProxyFromSettings` extension to wire it all up.

```csharp
builder.Services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(
    builder.Configuration.GetSection("MyApi")
)
.WithProxyFromSettings(settings => settings.Proxy); // Selects the ProxySettings object
```

This elegant design ensures that the proxy is configured correctly on the primary message handler, which is a requirement for 
[`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).