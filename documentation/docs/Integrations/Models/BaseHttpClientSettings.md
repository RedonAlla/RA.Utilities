---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations.Models
```

The `BaseHttpClientSettings<T>` class is a **ready-to-use base class for HTTP client configuration**.
It implements [`IIntegrationSettings`](../Abstractions/IIntegrationSettings.md) and provides the essential properties required to configure an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) — base URL, proxy behavior, and timeout — plus a strongly-typed container for the API's endpoint paths.

## 🎯 Purpose

The class exists so that every integration in your application shares one configuration shape:

- **Consistency** — every client settings class exposes `BaseUrl`, `Actions`, `UseProxy`, and `Timeout`, regardless of which API it targets.
- **Reusability** — extension methods like [`AddHttpClientIntegration`](../Extensions/DependencyInjectionExtensions.md#addhttpclientintegration) operate on any `IIntegrationSettings` implementation, including this class.
- **Strongly-typed endpoints** — the generic `Actions` property holds a dedicated class with the API's endpoint paths, giving IntelliSense and compile-time checking instead of "magic strings".
- **Validation support** — `BaseUrl` and `Actions` are marked `[Required]`, so binding through the options pattern with data annotation validation fails fast on missing configuration.

## ⚙️ Properties

| Property | Type | Description | Default |
| -------- | ---- | ----------- | ------- |
| **BaseUrl** | `Uri` | The base address for all requests made by the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). Marked `[Required]`. | `null` (must be provided) |
| **Actions** | `T` | A strongly-typed container holding the endpoint paths of the API. Marked `[Required]`. | `null` (must be provided) |
| **UseProxy** | `bool` | A flag indicating whether requests should be sent through a configured proxy. | `false` |
| **Timeout** | `double` | The request timeout in seconds. | `100` |

## 🚀 Example Usage

### 1. Define the settings class

```csharp showLineNumbers
using RA.Utilities.Integrations.Models;

public class WeatherApiSettings : BaseHttpClientSettings<WeatherApiActions>
{
}

public class WeatherApiActions
{
    public string GetCurrent { get; set; } = "current";
    public string GetForecast { get; set; } = "forecast/{0}";
}
```

### 2. Bind it from `appsettings.json`

```json
{
  "WeatherApi": {
    "BaseUrl": "https://api.weather.com/v1/",
    "Timeout": 30,
    "Actions": {
      "GetCurrent": "current",
      "GetForecast": "forecast/{0}"
    }
  }
}
```

### 3. Register the typed client

```csharp showLineNumbers
using RA.Utilities.Integrations.Extensions;

services.AddHttpClientIntegration<IWeatherApiClient, WeatherApiClient, WeatherApiSettings>(
    builder.Configuration.GetSection("WeatherApi"));
```

### 4. Use the strongly-typed endpoints

```csharp showLineNumbers
public class WeatherApiClient : BaseHttpClient
{
    private readonly WeatherApiActions _actions;

    public WeatherApiClient(HttpClient httpClient, IOptions<WeatherApiSettings> settings)
        : base(httpClient, settings)
    {
        _actions = settings.Value.Actions;
    }

    public Task<Weather?> GetCurrentAsync() =>
        GetAsync<Weather>(_actions.GetCurrent);
}
```

## 🆚 `BaseHttpClientSettings<T>` vs `BaseApiSettings<T>`

The package offers two settings base classes; pick based on how strict you want configuration to be:

| Aspect | `BaseHttpClientSettings<T>` (this class) | [`BaseApiSettings<T>`](../Options/BaseApiSettings.md) |
|---|---|---|
| Kind | Concrete class | Abstract class |
| Validation | `[Required]` on `BaseUrl` and `Actions` | `[Required]` + `[Url]` on `BaseUrl`, `[Range(1, 600)]` on `Timeout` |
| Language-level enforcement | No | `required` properties (`BaseUrl`, `Actions`) |
| Default timeout | `100` seconds | `200` seconds |
| Use proxy | `false` | `false` |
| Namespace | `RA.Utilities.Integrations.Models` | `RA.Utilities.Integrations.Options` |

Both implement `IIntegrationSettings`, so both work identically with `AddHttpClientIntegration`.

## 📋 Notes

- The class is **not abstract** — you can instantiate it directly when you don't need extra integration-specific settings.
- `Actions` can be any reference type; strings or dedicated endpoint classes are the most common choices.
- Since v10.1.0, media type and encoding are no longer part of the settings model — request bodies are always sent as `application/json; charset=utf-8`.

## 🧠 Summary

`BaseHttpClientSettings<T>` is the pragmatic, minimal-friction settings base: inherit it, bind it from configuration, and pass it to `AddHttpClientIntegration` — the package handles validation, client configuration, and endpoint typing from there.
