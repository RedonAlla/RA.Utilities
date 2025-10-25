```bash
Namespace: RA.Utilities.Integrations.DelegatingHandlers
```

The `ApiKeyAuthenticationHandler<TSettings>` is a specialized piece of middleware for
[`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) known as a
[`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler).
Its primary purpose is to automate the process of adding an API key to outgoing HTTP requests.

By intercepting every request sent by an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
it's attached to, this handler performs the following actions:

1. **Retrieves Configuration**:
It accesses a strongly-typed settings object (specified by the generic `TSettings` parameter)
that has been registered in the dependency injection container.
2. **Reads the API Key**:
It looks for an ApiKey property on that settings object, as defined by the [`IApiKeySettings`](../Abstractions/IApiKeySettings.md) interface.
3. **Injects the Header**:
If a valid API key is found, it automatically adds it to the request's headers under the name `X-Api-Key`.
4. **Passes the Request On**:
It then passes the modified request to the next handler in the pipeline, or to the server if it's the last handler.

The key benefit is **abstraction and convenience**.
It allows developers to configure an API key once during application startup and have it automatically applied to every call for that specific integration. This eliminates repetitive, error-prone code where developers would otherwise have to manually add the authentication header for each request.

## ⚙️ How It's Used
This handler is designed to be "plugged in" to an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
pipeline using the provided extension methods.

### 1. Define Settings:
A developer creates a settings class for a specific API that implements
[IIntegrationSettings](../Abstractions/IIntegrationSettings.md) and [`IApiKeySettings`](../Abstractions/IApiKeySettings.md).

```csharp
public class MyExternalApiSettings : IIntegrationSettings, IApiKeySettings
{
    public Uri BaseUrl { get; set; }
    public string ApiKey { get; set; }
    // ... other properties
}
```

### 2. Configure [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient):
In `Program.cs` or a service registration module, the developer registers the client and attaches the handler.

```csharp
services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyExternalApiSettings>(
    configuration.GetSection("MyExternalApi")
)
// highlight-next-line
.WithApiKeyFromSettingsHandler<MyExternalApiSettings>(); // <-- This adds the handler
```

The `WithApiKeyFromSettingsHandler` extension method (found in `DependencyInjectionExtensions.cs`) registers `ApiKeyAuthenticationHandler<MyExternalApiSettings>` and adds it to the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)'s message handler pipeline.
From that point on, any call made using IMyApiClient will automatically have the `X-Api-Key` header attached.