```bash
Namespace: RA.Utilities.Integrations.Options
```

The `BaseApiSettings<T>` class serves as a
**standardized and reusable abstract base class for creating strongly-typed configuration objects for external API integrations**.

It's a foundational component designed to work with the .NET configuration and options patterns,
where it would typically be populated from a section in `appsettings.json`.

## Here's a breakdown of its key purposes and design features:

### 1. Enforces a Standard Contract: 
By implementing the [`IIntegrationSettings`](../Abstractions/IIntegrationSettings.md) interface,
it guarantees that any concrete settings class inheriting from it will have a consistent set of essential properties required to configure an [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient), such as `BaseUrl`, `Timeout`, and `MediaType`.
This allows your integration helpers and [`HttpClientHandler`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler)
to work with any integration in a generic way.

### 2. Promotes Strongly-Typed Endpoints:
The generic parameter `T` for the `Actions` property is a standout feature.
It allows you to define a dedicated class containing the specific endpoint paths for an API.
This is a significant improvement over using "magic strings" for URLs throughout your codebase, as it provides:

  * **IntelliSense and Autocompletion**.
  * **Compile-time checking** against typos.
  * A single, clear location for all of an API's endpoints.

### 3. Provides Built-in Validation:
The class uses Data Annotations like `[Required]`, `[Url]`, and `[Range]`.
When these settings are bound from configuration at application startup, the .NET Options validation system can automatically check them.
This ensures that the application will fail fast with a clear error if the configuration is missing or invalid, preventing potential runtime errors.

### 4. Reduces Boilerplate Code:
It provides sensible defaults for common settings like `Timeout`, `MediaType`, and `Encoding`.
This means developers creating a new settings class for a specific API only need to define what's unique to that integration, inheriting all the common functionality.


## Properties

This class serves as a foundational configuration model for setting up 
[`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
instances for various API integrations.

| Property |	Type	| Description	| Default Value |
| -------- | ------ | ----------- | ------------- |
| **BaseUrl** | `Uri` |	The base address for the API. This is a required property and must be a valid URL. | `null` |
| **Actions** | `T` |	A container object holding the strongly-typed endpoint paths for the specific API. This is a required property.	| `null` |
| **UseProxy** | `bool` |	A flag indicating whether requests should be sent through a configured proxy.	| `false` |
| **Timeout** | `double` |	The request timeout in seconds. The value must be between 1 and 600.	| `200` |
| **MediaType** | `string` |	The default Content-Type header value for requests (e.g., for POST/PUT bodies).	| `"application/json"` |
| **Encoding** | `string` |	The default character encoding for request content.	| `"utf-8"` |


In essence, `BaseApiSettings<T>` is the cornerstone of your `RA.Utilities.Integrations` package.
It establishes a robust, consistent, and developer-friendly pattern for managing the configuration of all external API clients.