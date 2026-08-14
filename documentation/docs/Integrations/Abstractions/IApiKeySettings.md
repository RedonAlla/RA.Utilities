---
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

The `IApiKeySettings` interface defines a **standardized contract for configuration classes that need to provide an API key for authentication**.
It declares a single property, `ApiKey`, that settings classes expose for the package's API key handlers to consume.

## 🔑 This serves a few key goals:

#### 1. Standardization:
It ensures that any integration requiring an API key has a consistent way of exposing that key from its settings. This is especially useful for creating reusable components or middleware that might need to access the key.

#### 2. Abstraction:
It allows other parts of the system to depend on the `IApiKeySettings` contract rather than a specific concrete settings class (like `MyApiSettings`).
This promotes loose coupling.

#### 3. Clarity and Intent:
It makes the configuration's purpose explicit.
When a settings class implements `IApiKeySettings`, it clearly signals that this integration authenticates using an API key.

## Properties
This interface defines a standardized contract for configuration classes that provide an API key for authentication.

| Property |	Type |	Description |
| -------- | ----- | ------------ |
| **ApiKey** |	`string?` |	Gets or sets the API key value. |

## ⚙️ How It Fits In
`IApiKeySettings` is consumed by the package's [`WithApiKeyFromSettingsHandler<TSettings>`](../Extensions/DependencyInjectionExtensions.md#withapikeyfromsettingshandlertsettings) fluent method, which injects the API key from your settings object into the `X-Api-Key` header of every request:

```csharp
public class MyApiSettings : BaseApiSettings<MyApiActions>, IApiKeySettings
{
    public string? ApiKey { get; set; }
}

services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(configSection)
    .WithApiKeyFromSettingsHandler<MyApiSettings>();
```

For a static API key you can also use [`WithApiKey(...)`](../Extensions/DependencyInjectionExtensions.md#withapikey):

```csharp
services.AddHttpClientIntegration<IMyApiClient, MyApiClient, MyApiSettings>(configSection)
    .WithApiKey("your-secret-api-key");
```

## 🧠 Summary
In short, `IApiKeySettings` standardizes API key management so that reusable handlers can authenticate any integration from its settings — self-documenting and consistent across your integrations.