---
title: Integration Layer
sidebar_position: 6
---

# Integration Layer

The Integration layer lives in `src/Infrastructure/RaTemplate.Integration` and owns **outgoing** HTTP communication — calling external APIs and other services. It implements client abstractions declared in Application, so handlers never touch `HttpClient` or vendor SDKs directly. The project is generated **only when `UseIntegrations` is `true`** (the default); with `--UseIntegrations false` it is excluded entirely.

:::info[Template state:]
the scaffold ships just the wiring — `DependencyInjection.cs` (registering the request/response logging handler) and `AssemblyReference.cs`.
You add your own typed clients, settings, and abstractions; the patterns below show how.
:::

## Project Dependencies

| Reference | Kind | Purpose |
| :--- | :--- | :--- |
| `RaTemplate.Application` | Project | The client abstractions this layer implements |
| `RA.Utilities.Integrations` | Package | Typed-client registration helpers, delegating handlers, `BaseHttpClient`, settings contracts, and query/header utilities |

## What the Template Registers

```csharp
public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
{
    services.AddScopedHttpMessageHandler<RequestResponseLoggingHandler>();
    return services;
}
```

`AddScopedHttpMessageHandler<T>()` registers a `DelegatingHandler` in DI with scoped lifetime so it can be attached to typed clients via `AddHttpMessageHandler<T>()` (or the `WithHttpLoggingHandler()` shortcut below). `AddIntegrationServices` is called by the Infrastructure aggregator (`AddInfrastructureServices`) only when the project exists.

## Request/Response Logging

`RequestResponseLoggingHandler` logs every outgoing call and its result as structured objects (`HttpRequestLogTemplate` / `HttpResponseLogTemplate` from `RA.Utilities.Logging.Shared`) at `Information`:

- **Request**: request id (from `x-request-id`, falling back to the current `HttpContext.TraceIdentifier`), scheme, host, method, full path, query string, headers, and body — deserialized as JSON when possible, otherwise logged as a raw string.
- **Response**: request id, trace identifier, path, status code, headers, duration, and body.

Logging is skipped entirely when `Information` is not enabled, and responses exceeding the handler's warning threshold escalate to `Warning`. Correlating the trace identifier with the API's own request logging (see the [Presentation Layer](presentation-layer.md)) gives end-to-end visibility across an inbound request and its outbound calls.

## Adding a Client

Follow the dependency rule: **declare the abstraction in Application, implement it here.**

```csharp
// RaTemplate.Application — the contract handlers depend on
public interface ICustomersClient
{
    Task<CustomerOutput?> GetCustomerAsync(Guid id, CancellationToken cancellationToken);
}
```

```csharp
// RaTemplate.Integration — the implementation
public sealed class CustomersClient(HttpClient httpClient) : ICustomersClient
{
    public async Task<CustomerOutput?> GetCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"customers/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CustomerOutput>(cancellationToken);
    }
}
```

Register it as a typed client with the package's helper, which binds a settings class from configuration, validates it with DataAnnotations **on startup** (`ValidateOnStart`), and configures `BaseAddress` and `Timeout` from the settings:

```csharp
services.AddHttpClientIntegration<ICustomersClient, CustomersClient, CustomersApiSettings>(
        configuration.GetSection("Integrations:CustomersApi"))
    .WithHttpLoggingHandler();
```

```jsonc
// appsettings.json
"Integrations": {
  "CustomersApi": {
    "BaseUrl": "https://api.example.com/",
    "Timeout": 30
  }
}
```

### Settings contracts

Settings classes implement `IIntegrationSettings` (`BaseUrl`, `UseProxy`, `Timeout` in seconds) — or derive from `BaseApiSettings<T>`, which adds DataAnnotation validation (`BaseUrl` required and a valid URL, `Timeout` in the 1–600 range, default 200) and a required `Actions` object holding the target endpoint names.

### Fluent handler helpers

`IHttpClientBuilder` extensions from `RA.Utilities.Integrations.Extensions`:

| Helper | Effect |
| :--- | :--- |
| `WithHttpLoggingHandler()` | Attaches `RequestResponseLoggingHandler` to this client |
| `WithInternalHeadersForwardingHandler()` | Attaches `InternalHeadersForwardHandler` — forwards the current access token and `x-request-id` on each call |
| `WithApiKey(apiKey, headerName = "X-Api-Key")` | Adds a static API key header |
| `WithApiKeyFromSettingsHandler<TSettings>()` | Attaches `ApiKeyAuthenticationHandler<TSettings>` — injects the key from settings implementing `IApiKeySettings` |
| `WithProxyFromSettings<TSettings>(selector)` | Configures the primary handler to route through a proxy defined by `IProxySettings` |

`AddTransientHttpMessageHandler<T>()` is also available for handlers that should not share a scope.

## BaseHttpClient (optional convenience)

The package ships a `BaseHttpClient` base class with `GetAsync` / `PostAsync` / `PutAsync` / `DeleteAsync` helpers that handle query-string building (from `IQueryStringRequest` objects via `QueryUtilities`, plus the `QueryParameters` / `QueryParameterName` attributes), header mapping (from `IHeaderRequest` via the `HeaderParameters` / `HeaderParameterName` attributes), JSON body serialization, `EnsureSuccessStatusCode()`, and response deserialization. Derive from it instead of using `HttpClient` directly when you want that plumbing for free; plain typed clients as shown above are equally valid.

## Recommended Layout

```text
RaTemplate.Integration/
    Clients/
        CustomersClient.cs
    Settings/
        CustomersApiSettings.cs
    DependencyInjection.cs
    AssemblyReference.cs
```

## DI Registration Flow

```text
Program.cs
  └─ AddInfrastructureServices(configuration)      (RaTemplate.Infrastructure)
       └─ AddIntegrationServices(configuration)    (RaTemplate.Integration, if enabled)
            ├─ RequestResponseLoggingHandler       (scoped delegating handler)
            └─ your typed clients                  (AddHttpClientIntegration<...>)
```

## Conditional Generation

- **`--UseIntegrations false`** → the whole `RaTemplate.Integration` project is excluded, the Infrastructure aggregator drops its project reference and the `AddIntegrationServices` call, and the architecture tests skip the Integration assembly.

## Dependencies

Integration depends on Application (for the client abstractions) and the `RA.Utilities.Integrations` package. Nothing in Application or Domain references it — handlers consume `ICustomersClient`-style interfaces, so any external provider can be swapped or faked without touching core logic. The `DependencyTests` enforce this direction.
