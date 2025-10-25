---
sidebar_position: 1
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

Its primary purpose is to ensure that all settings classes used to configure [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) instances through the `RA.Utilities.Integrations` package adhere to a standardized set of properties.

This standardization is crucial for:

#### 1. Consistency:
It guarantees that every integration, regardless of its specific API, will expose common configuration elements like a base URL, timeout, and media type.

#### 2. Reusability:
Components and extension methods (like `AddHttpClientIntegration`) can operate on any object implementing `IIntegrationSettings`,
making them generic and reusable across different integrations.

#### 3. Decoupling:
It allows the integration infrastructure to depend on an abstract contract rather than concrete settings classes,
promoting loose coupling and easier maintenance.

#### 4. Clarity:
It clearly defines the essential parameters needed to set up an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) for external API communication.


## Properties
This interface defines a standardized contract for configuration classes that provide an API key for authentication.

| Property |	Type |	Description |
| -------- | ----- | ------------ |
| **BaseUrl** |	`Uri` |	The base address for all requests made by the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient). This is a required property. |
| **UseProxy** |	`bool` |	A flag to determine if the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient) should route requests through a configured proxy. Defaults to `false`. |
| **Timeout** |	`double` |	The number of seconds to wait for a response before the request times out. The default is 100 seconds. |
| **MediaType** |	`string` |	The media type (e.g., `"application/json"`) used for the Content-Type header in requests with a body. |
| **Encoding** |	`string` |	The character encoding (e.g., `"utf-8"`) used with the `MediaType` to form the complete `Content-Type` header. |
