---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Extensions
```

The `ServiceCollectionExtensions` class is the primary entry point for setting up [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) integrations within the .NET dependency injection container.
Its methods are declared as **C# 14 extension members** on [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) (inside an `extension(IServiceCollection)` block), so they are called with the familiar `services.Method(...)` syntax.

## `AddHttpClientIntegration<...>()`

This is the main method for registering a typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).
It binds configuration from `appsettings.json`, validates it on startup, and configures the client's `BaseAddress` and `Timeout`.

#### Parameters

| Parameter | Type | Description |
| :--- | :--- | :--- |
| `services` | [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) | The [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) to add the services to. |
| `configurationSection` | [`IConfigurationSection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationsection) | The configuration section (e.g., from `appsettings.json`) to bind the settings from. |
| `TInterface` | `class` | The interface of the typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) to register. |
| `TClient` | `class, TInterface` | The concrete implementation of the typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). |
| `TSettings` | `class, [IIntegrationSettings](../Abstractions/IIntegrationSettings.md)` | The settings class that holds the configuration for this integration. |
| **Returns** | [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) | An [`IHttpClientBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) that can be used to further configure the client pipeline — see [`HttpClientBuilderExtensions`](./HttpClientBuilderExtensions.md). |

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

:::info[v10.2.0 class rename]
These members were previously declared in a single `DependencyInjectionExtensions` class. In v10.2.0 that class was split into `ServiceCollectionExtensions` (the methods above) and [`HttpClientBuilderExtensions`](./HttpClientBuilderExtensions.md) (the pipeline methods), and the members were refactored to C# 14 extension members. Method names, signatures, and the `RA.Utilities.Integrations.Extensions` namespace are unchanged — see the [migration guide](../migration-guides) for details.
:::
