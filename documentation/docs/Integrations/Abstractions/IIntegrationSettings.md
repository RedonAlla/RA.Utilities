---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

Its primary purpose is to ensure that all settings classes used to configure [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) instances through the `RA.Utilities.Integrations` package adhere to a standardized set of properties.

This standardization is crucial for:

#### 1. Consistency:
It guarantees that every integration, regardless of its specific API, will expose common configuration elements like a base URL, timeout, and proxy behavior.

#### 2. Reusability:
Components and extension methods (like [`AddHttpClientIntegration`](../Extensions/ServiceCollectionExtensions.md#addhttpclientintegration)) can operate on any object implementing `IIntegrationSettings`,
making them generic and reusable across different integrations.

#### 3. Decoupling:
It allows the integration infrastructure to depend on an abstract contract rather than concrete settings classes,
promoting loose coupling and easier maintenance.

#### 4. Clarity:
It clearly defines the essential parameters needed to set up an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) for external API communication.


## Properties
This interface defines a standardized contract for configuration classes used to set up an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) integration.

| Property |	Type |	Description |
| -------- | ----- | ------------ |
| **BaseUrl** |	`Uri` |	The base address for all requests made by the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). This is a required property. |
| **UseProxy** |	`bool` |	A flag indicating whether requests should be routed through a proxy. Defaults to `false`. Since v10.2.0 no built-in component consumes this flag (the proxy pipeline was removed) — it remains on the contract for compatibility; see the [migration guide](../migration-guides). |
| **Timeout** |	`double` |	The number of seconds to wait for a response before the request times out. Implementations choose their own default — [`BaseApiSettings<T>`](../Options/BaseApiSettings.md) uses `200`. |

Since v10.1.0 request bodies are always serialized as JSON (`application/json; charset=utf-8`); the interface no longer carries `MediaType` or `Encoding` properties.

## 🧠 Summary

`IIntegrationSettings` is the minimal contract that makes the whole integration pipeline generic: any settings object implementing it can be bound from configuration, validated at startup, and used to configure a typed [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) — see [`BaseApiSettings<T>`](../Options/BaseApiSettings.md) for a ready-made base class.
