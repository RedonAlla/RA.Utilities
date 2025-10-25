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
* **Accept Header**: The media type the client expects in the response (e.g., `application/json`).
* **Timeout**: A default timeout for all requests.

This configuration is driven by the [`IIntegrationSettings`](./Abstractions/IIntegrationSettings.md) interface,
meaning you can easily create different clients for different APIs by just providing a different settings implementation.

```csharp showLineNumbers
// Constructor
public BaseHttpClient(HttpClient httpClient, IOptions<IIntegrationSettings> settings)
{
    _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    // Configuration happens here
    _httpClient.BaseAddress = _settings.BaseUrl;
    _httpClient.DefaultRequestHeaders.Add("Accept", _settings.MediaType);
    _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
}
```

### 2. Abstraction of HTTP Operations
It provides simple, high-level methods for common HTTP verbs like `GET`, `POST`, and `PUT`.
This hides the lower-level details of creating [HttpRequestMessage](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestmessage), serializing request bodies, and deserializing responses.

For example, instead of manually building a request every time, a developer can just call:
```csharp showLineNumbers
// Instead of complex manual setup, just call this:
var user = await myApiClient.GetAsync<User>($"users/{userId}");
```

### 3. Automatic Serialization and Deserialization
The class offers generic method overloads that automatically handle the conversion between C# objects and JSON strings.

When sending data (e.g., in a `POST` or `PUT` request), it serializes the request body object into a JSON string.
When receiving data, it deserializes the JSON response string back into a strongly-typed C# object (`TResponse`).

This is handled by the `JsonConverterUtilities`s and is evident in methods like `GetAsync<TResponse>`:
```csharp showLineNumbers
public async Task<TResponse> GetAsync<TResponse>(...) where TResponse : class
{
    // 1. Gets the raw string response from another overload
    string response = await GetAsync(action, queryString, headers, cancellationToken);
    
    // 2. Deserializes the string into the desired object type
    return JsonConverterUtilities.ToObject<TResponse>(response, _settings.MediaType)!;
}
```

### 4. Code Reusability and Consistency
The private `BaseHttpCall` method centralizes the core logic for sending any request.
This ensures that all requests (`GET`, `POST`, `PUT`) are built and handled consistently, reducing code duplication and the chance of errors. It handles:

* Building the final request URL with query parameters.
* Adding custom headers.
* Attaching the serialized request body.
* Sending the request.
* Checking for a successful HTTP status code (`response.EnsureSuccessStatusCode()`), which throws an exception if the request failed.


## 🧠 Summary
In summary, `BaseHttpClient` **is a foundational building block for creating robust, reusable, and easy-to-use API clients**.
It promotes best practices like dependency injection and abstracts away the repetitive boilerplate code associated with making HTTP calls, allowing developers to focus on the business logic of their application.