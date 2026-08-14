---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations
```

The `BaseHttpClient` class serves as a reusable and configurable wrapper around .NET's [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient).
Its main purpose is to simplify and standardize the process of making HTTP requests to a specific external API.

## Here's a breakdown of its key responsibilities:

### 1. Centralized Configuration
The class is configured once during its creation using dependency injection.
In the constructor, it sets up the [`HttpClient`](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
instance with common properties required for a specific API integration:

* **BaseAddress**: The root URL for all API calls (e.g., `https://api.example.com/`).
* **Timeout**: A default timeout for all requests.

This configuration is driven by the [`IIntegrationSettings`](./Abstractions/IIntegrationSettings.md) interface,
meaning you can easily create different clients for different APIs by just providing a different settings implementation.

```csharp showLineNumbers
// Constructor
public BaseHttpClient(HttpClient httpClient, IOptions<IIntegrationSettings> settings)
{
    IIntegrationSettings settingsValue = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    // Configuration happens here
    _httpClient.BaseAddress = settingsValue.BaseUrl;
    _httpClient.Timeout = TimeSpan.FromSeconds(settingsValue.Timeout);
}
```

### 2. Abstraction of All Four HTTP Verbs
It provides simple, high-level methods for the complete CRUD surface — `GET`, `POST`, `PUT`, and `DELETE`.
This hides the lower-level details of creating [HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage), serializing request bodies, and deserializing responses.

Every verb offers two overloads: one that returns the raw response as a `string`, and one that deserializes it into a strongly-typed `TResponse` (`null` when the response body is empty).

```csharp showLineNumbers
// Instead of complex manual setup, just call these:
Product? product = await myApiClient.GetAsync<Product>($"products/{id}");
Product? created = await myApiClient.PostAsync<Product, Product>("products", product);
Product? updated = await myApiClient.PutAsync<Product, Product>($"products/{id}", product);
await myApiClient.DeleteAsync($"products/{id}");
```

### 3. Strongly-Typed Query String and Header Parameters
`GET` and `DELETE` (and optionally `POST`/`PUT`) accept a query string object implementing [`IQueryStringRequest`](./Abstractions/IQueryStringRequest.md)
and a headers object implementing [`IHeaderRequest`](./Abstractions/IHeaderRequest.md) — both optional, and `null` is handled gracefully.

Mark a `partial` class with [`[QueryParameters]`](./Attributes/QueryParametersAttribute.md) or [`[HeaderParameters]`](./Attributes/HeaderParametersAttribute.md)
and the source generator implements the contract for you, mapping each public property to a key-value pair:

```csharp showLineNumbers
[QueryParameters]
public partial class GetProductsQuery
{
    public int? CategoryId { get; init; }
    public string? Search { get; init; }
}

[HeaderParameters]
public partial class RequestHeaders
{
    [HeaderParameterName("x-request-id")]
    public string? XCorrelationId { get; init; }
}

// GET /products?CategoryId=3&Search=widgets with an x-request-id header
Product? product = await client.GetAsync<Product>(
    "products",
    new GetProductsQuery { CategoryId = 3, Search = "widgets" },
    new RequestHeaders { XCorrelationId = "trace-1" });
```

The generated query mapping skips `null` values, formats `bool` as `true`/`false`, `DateTime`/`DateTimeOffset` with the round-trip format, and emits collections as repeated keys.

### 4. Automatic Serialization and Deserialization
When sending data (e.g., in a `POST` or `PUT` request), the request body object is serialized into a JSON string with camel-case naming and the `application/json; charset=utf-8` content type.
When receiving data, the JSON response string is deserialized back into a strongly-typed C# object (`TResponse`).

This is handled by the `JsonConverterUtilities` and is evident in methods like `GetAsync<TResponse>`:

```csharp showLineNumbers
public async Task<TResponse?> GetAsync<TResponse>(...) where TResponse : class
{
    // 1. Gets the raw string response from another overload
    string response = await GetAsync(action, queryString, headers, cancellationToken);

    // 2. Deserializes the string into the desired object type
    return JsonConverterUtilities.ToObject<TResponse>(response);
}
```

### 5. Code Reusability and Consistency
The private `BaseHttpCall` method centralizes the core logic for sending any request.
This ensures that all requests (`GET`, `POST`, `PUT`, `DELETE`) are built and handled consistently, reducing code duplication and the chance of errors. It handles:

* Building the final request URL with query parameters (via [`QueryUtilities`](./Utilities/QueryUtilities.md)).
* Adding custom headers.
* Attaching the serialized request body.
* Sending the request.
* Checking for a successful HTTP status code (`response.EnsureSuccessStatusCode()`), which throws an exception if the request failed.


## 🧠 Summary
In summary, `BaseHttpClient` **is a foundational building block for creating robust, reusable, and easy-to-use API clients**.
It promotes best practices like dependency injection and abstracts away the repetitive boilerplate code associated with making HTTP calls, allowing developers to focus on the business logic of their application.
