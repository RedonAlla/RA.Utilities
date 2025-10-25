---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Extensions
```

The `DependencyInjectionExtensions` class is the primary entry point for setting up [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) integrations within the .NET dependency injection container.
It provides a fluent API to simplify and standardize the registration and configuration of typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)s, encapsulating best practices and reducing boilerplate code in your `Program.cs` file.

## `AddHttpClientIntegration<...>()`

This is the main extension method for registering a typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).
It binds configuration from `appsettings.json`, validates it on startup, and configures the client's `BaseAddress` and `Timeout`.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `services` | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the services to. |
| `configurationSection` | [`IConfigurationSection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationsection) | The configuration section (e.g., from `appsettings.json`) to bind the settings from. |
| `TInterface` | `class` | The interface of the typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) to register. |
| `TClient` | `class, TInterface` | The concrete implementation of the typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). |
| `TSettings` | `class, [IIntegrationSettings](../Abstractions/IIntegrationSettings.md)` | The settings class that holds the configuration for this integration. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | An [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) that can be used to further configure the client pipeline. |

#### Example

This example shows how to register a `WeatherApiClient` with its settings.

1.  **`appsettings.json` configuration:**

```json
{
  "WeatherApi": {
    "BaseUrl": "https://api.weather.com/v1/",
    "Timeout": 30,
    "Actions": {
      "GetCurrent": "current"
    }
  }
}
```

2.  **Define Settings and Client classes:**

```csharp
// Settings classes
public class WeatherApiSettings : BaseApiSettings<WeatherApiActions> { }
public class WeatherApiActions
{
    public required string GetCurrent { get; set; }
}

// Client interface and implementation
public interface IWeatherApiClient
{
    Task<string> GetCurrentWeatherAsync(string city);
}

public class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly WeatherApiSettings _settings;

    public WeatherApiClient(HttpClient httpClient, IOptions<WeatherApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string> GetCurrentWeatherAsync(string city)
    {
        var endpoint = $"{_settings.Actions.GetCurrent}?city={city}";
        return await _httpClient.GetStringAsync(endpoint);
    }
}
```

3.  **Register in `Program.cs`:**

```csharp
// In Program.cs
using RA.Utilities.Integrations.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClientIntegration<IWeatherApiClient, WeatherApiClient, WeatherApiSettings>(
    builder.Configuration.GetSection("WeatherApi")
);
```

---

## `AddOptionWithValidations<T>()`

Registers a configuration class from `appsettings.json` and enables data annotation validation on application startup.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `services` | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the services to. |
| `key` | `string?` | The configuration key to bind from. If `null`, it defaults to the class name of `T`. |
| `T` | `class` | The options class to register and validate. |

#### Example

```csharp
// In Program.cs
using RA.Utilities.Integrations.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Assumes a "MyFeature" section exists in appsettings.json
builder.Services.AddOptionWithValidations<MyFeatureSettings>();

// Or with an explicit key
builder.Services.AddOptionWithValidations<MyFeatureSettings>("MyFeature");
```

---

## `AddScopedHttpMessageHandler<T>()`

Registers a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) with a scoped lifetime in the DI container.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `services` | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the services to. |
| `T` | [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) | The handler type to register. |
| **Returns** | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The original [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddScopedHttpMessageHandler<MyCustomHandler>();
```

---

## `AddTransientHttpMessageHandler<T>()`

Registers a [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) with a transient lifetime in the DI container. This is the most common lifetime for handlers.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `services` | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the services to. |
| `T` | [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler) | The handler type to register. |
| **Returns** | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The original [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddTransientHttpMessageHandler<MyCustomHandler>();
```

---

## `With...()` Fluent Extensions

These methods chain off `AddHttpClientIntegration` to add [`DelegatingHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.delegatinghandler)s or other configurations to the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) pipeline.

### `WithHttpLoggingHandler()`

Adds the `RequestResponseLoggingHandler` to log request and response details.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpClientBuilder` | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The builder to configure. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The original builder for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddTransientHttpMessageHandler<RequestResponseLoggingHandler>(); // Must be registered first

builder.Services.AddHttpClientIntegration<...>(...)
    .WithHttpLoggingHandler();
```

### `WithInternalHeadersForwardingHandler()`

Adds the `InternalHeadersForwardHandler` to forward headers like `X-Request-Id` and the user's `Authorization` token.

#### Example

```csharp
// In Program.cs
builder.Services.AddTransientHttpMessageHandler<InternalHeadersForwardHandler>(); // Must be registered first

builder.Services.AddHttpClientIntegration<...>(...)
    .WithInternalHeadersForwardingHandler();
```

### `WithApiKey(...)`

Adds a static API key directly to the client's default request headers.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpClientBuilder` | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The builder to configure. |
| `apiKey` | `string` | The API key value. |
| `headerName` | `string` | The name of the header (defaults to "X-Api-Key"). |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The original builder for chaining. |

#### Example

```csharp
// In Program.cs
builder.Services.AddHttpClientIntegration<...>(...)
    .WithApiKey("my-secret-api-key", "Authorization");
```

### `WithApiKeyFromSettingsHandler<TSettings>()`

Adds a handler that dynamically injects an API key from a settings class that implements `IApiKeySettings`.

#### Example

```csharp
// Define a settings class with the API key
public class MyApiSettings : BaseApiSettings<MyApiActions>, IApiKeySettings
{
    public required string ApiKey { get; set; }
}

// In Program.cs
builder.Services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(...)
    .WithApiKeyFromSettingsHandler<MyApiSettings>();
```

### `WithProxyFromSettings<TSettings>()`

Configures the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) to use a proxy, with settings extracted from a configuration object.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `httpClientBuilder` | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The builder to configure. |
| `proxySettingsSelector` | `Func<TSettings, IProxySettings>` | A function to select the proxy settings from the root settings object. |
| `TSettings` | `class` | The root settings class. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | The original builder for chaining. |

#### Example

```csharp
// Define a settings class with nested proxy settings
public class MyApiSettings : BaseApiSettings<MyApiActions>
{
    // This property will be populated from the "Proxy" section in appsettings.json
    public required ProxySettings Proxy { get; set; }
}

// In Program.cs
builder.Services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(...)
    .WithProxyFromSettings(settings => settings.Proxy);
```