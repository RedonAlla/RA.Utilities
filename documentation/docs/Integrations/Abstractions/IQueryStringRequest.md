---
sidebar_position: 2
---

```bash
Namespace: RA.Utilities.Integrations.Abstractions
```

The `IQueryStringRequest` interface provides a standardized contract for request models that need to be serialized into a URL query string.
Its main goal is to create a clean, reusable, and type-safe pattern for defining and using parameters in HTTP `GET` requests.

This approach is central to the `RA.Utilities.Integrations` package for several reasons:

1. **Standardization**:
It ensures that every request model providing query parameters does so consistently.
Components like `BaseHttpClient` can operate on any object implementing this interface without needing to know its specific properties.
2. **Type Safety**:
Instead of manually building URL strings or passing dictionaries, you create strongly-typed classes (e.g., `GetUserRequest`) that represent the available parameters.
This reduces runtime errors and improves code readability.
3. **Encapsulation**:
The request model itself contains the logic for how its properties map to query string keys and values.
The complex work of URL encoding, handling nulls, and formatting is abstracted away.
4. **Decoupling**:
It decouples the HTTP client from the concrete request models.
The client depends on the `IQueryStringRequest` abstraction, making it generic and reusable across any API integration.


## ⚙️ How It Works
The interface is designed for simplicity.
A developer only needs to implement the `QueryStringValues` method, while the more complex `ToQueryString` logic is provided by a default interface implementation.

| Method |	Return | Type	Description |
| ------ | ------- | ---------------- |
| **QueryStringValues()** |	`QueryParams` |	**(Must be implemented)**. Converts the properties of your request object into a collection of key-value pairs. |
| **ToQueryString(string action)** |	`string` |	**(Default implementation)**. Consumes the result of `QueryStringValues()` to build a complete, URL-encoded query string appended to the base `action` path. |

## 🚀 Example Usage
Imagine an API endpoint `/users` that accepts `status` and `page` as query parameters.

### 1. Define the Request Model

Create a class that implements `IQueryStringRequest`.
The `QueryStringValues` method maps the class properties to query parameters.

```csharp showLineNumbers
public class GetUsersRequest : IQueryStringRequest
{
    public string? Status { get; set; }
    public int? Page { get; set; }

    public QueryParams QueryStringValues()
    {
        var query = new QueryParams();
        query.AddIfNotNull(nameof(Status), Status);
        query.AddIfNotNull(nameof(Page), Page);
        return query;
    }
}
```

### 2. Use it with the HTTP Client
Instantiate your request model and pass it to the client's `GetAsync` method.
The framework handles the rest.

```csharp
// In your application service...
var request = new GetUsersRequest { Status = "active", Page = 2 };

// The client's GetAsync method accepts IQueryStringRequest
var users = await _myApiClient.GetAsync<List<User>>("users", request);
```

The `BaseHttpClient` will automatically call `request.ToQueryString("users")` to generate the final URL: `https://api.example.com/users?Status=active&Page=2`.

## 🧠 Summary
In short, `IQueryStringRequest` is a powerful abstraction that streamlines the creation of query strings.
It promotes clean, type-safe, and reusable code by standardizing how request models are serialized into URLs,
which is a cornerstone of the `RA.Utilities.Integrations` framework.