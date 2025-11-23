# RA.Utilities.Logging.Shared

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Logging.Shared?logo=nuget&label=NuGet)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Logging.Shared.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Logging.Shared/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Logging/Shared/)


`RA.Utilities.Logging.Shared` provides shared components and abstractions for logging HTTP requests and responses within the RA.Utilities ecosystem.
This package contains core models and helpers used by other logging libraries, such as `RA.Utilities.Api.Middlewares`, to ensure a consistent approach to capturing and structuring diagnostic information for HTTP calls.

## 📚 Table of Contents

- Purpose
- Installation
- Core Components
- Contributing

## Purpose

This package is a foundational library and is not intended for direct use in most applications.
Its primary role is to provide a common set of data structures (models) that other `RA.Utilities` packages can rely on for logging HTTP traffic.

By centralizing these models, we ensure that any middleware or service that logs HTTP requests and responses does so in a consistent, structured format.

## 🛠️ Installation

While you typically won't need to install this package directly, it is available on NuGet.
It will be included automatically when you install a dependent package like `RA.Utilities.Api.Middlewares`.

```bash
dotnet add package RA.Utilities.Logging.Shared
```

---

## Core Components

This package provides the data models used to structure log information for HTTP interactions.

### `BaseHttpLogTemplate`

```csharp
public class BaseHttpLogTemplate
```

This is the primary model used to capture a complete picture of an HTTP request-response cycle.

#### Properties
Base containing HTTP request response properties for logging.

| Name | Type | Required | Description |
| ---- | ---- | -------- | ----------- |
| RequestId.            | `string`            | Unique identifier to represent this request in trace logs. |
| TraceIdentifier | `string` | **false** | Unique identifier to represent this request in trace logs. Value of [`HttpContext.TraceIdentifier`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.traceidentifier?view=aspnetcore-8.0) |
| Path | `string` | **false** | The URI used by the request message. |
| RequestedOn | `DateTime` | **true** | DateTime requested on. Default value `DateTime.UtcNow` |
| RemoteAddress | `string` | **false** | The host name requested. This is usually the DNS host name or IP address of the server. |

### HttpRequestLogTemplate
Model for logging HTTP request.

```csharp
public class HttpRequestLogTemplate : BaseHttpLogTemplate
```

> [!NOTE]  
> **HttpRequestLogTemplate** inherits from [BaseHttpLogTemplate] so it will have all properties of [BaseHttpLogTemplate] class.

| Name | Type | Required | Description |
| ---- | ---- | -------- | ----------- |
| Schema | `string` | **false** | Scheme string of request Uri used by the request message. |
| Method | `string` | **false** | The HTTP method used by the request message. |
| QueryString | `string` | **false** | The query string used by the request message. |
| RequestHeaders | [`IDictionary<string, string>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2?view=net-8.0) | **false** | Collection of HTTP request headers used by the request message. |
| RequestBody | `object` | **false** | HTTP Request body. |
	

### HttpResponseLogTemplate
Model for logging HTTP response.

```csharp
public class HttpResponseLogTemplate : BaseHttpLogTemplate
```

> [!NOTE]  
> **HttpResponseLogTemplate** inherits from [BaseHttpLogTemplate] so it will have all properties of [BaseHttpLogTemplate] class.

| Name | Type | Required | Description |
| ---- | ---- | -------- | ----------- |
| StatusCode | `int` | **true** | The status code of the HTTP response. |
| ResponseHeaders | [`IDictionary<string, string>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2?view=net-8.0) | **false** | Collection of HTTP response headers. |
| ResponseBody | `object` | **false** | HTTP Request body. |

---

## Contributing

Contributions are welcome! If you have a suggestion or find a bug, please open an issue to discuss it. Please follow the contribution guidelines outlined in the other `RA.Utilities` packages.
