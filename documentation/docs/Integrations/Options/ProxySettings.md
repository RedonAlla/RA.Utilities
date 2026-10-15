```bash
Namespace: RA.Utilities.Integrations.Options
```

The `ProxySettings` class is a **strongly-typed configuration model** designed to hold the settings of an HTTP proxy server:
the proxy address and, optionally, the credentials used to authenticate against it.

## Properties

| Property | Type | Description |
| --------- | ----- | ----------- |
| **Address** | `string?` | The URI of the proxy server (e.g., `"http://proxy.example.com:8888"`). |
| **Username** | `string?` | The username for proxy authentication, if required. |
| **Password** | `string?` | The password for proxy authentication, if required. |
| **BypassProxyOnLocal** | `bool` | A flag indicating whether the proxy should be bypassed for local (intranet) addresses. |

It is a plain POCO (Plain Old C# Object), so the .NET configuration system can bind it directly from a configuration section such as:

```json showLineNumbers
"Proxy": {
  "Address": "http://proxy.server:8080",
  "Username": "proxy-user",
  "Password": "proxy-password",
  "BypassProxyOnLocal": true
}
```

```csharp showLineNumbers
public class MyApiSettings : BaseApiSettings<MyApiActions>
{
    public ProxySettings? Proxy { get; set; }
}
```

:::warning[v10.2.0 proxy pipeline removal]
In v10.1.1 this class implemented the `IProxySettings` contract and was consumed by the built-in proxy pipeline (`ProxyMessageHandler` factory + `WithProxyFromSettings<TSettings>()` extension). That entire pipeline was removed in v10.2.0 — the class remains as a configuration-only POCO, and no package component reads `ProxySettings` (or the `UseProxy` flag on `IIntegrationSettings`/`BaseApiSettings<T>`) anymore. To route a typed client through a proxy now, configure the primary handler yourself, e.g. `ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { Proxy = new WebProxy(...) })`. See the [migration guide](../migration-guides).
:::
