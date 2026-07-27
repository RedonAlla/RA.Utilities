---
title: ResponseType
---

```bash
Namespace: RA.Utilities.Core.Constants
```

The `ResponseType` is an implementation of the **type-safe enum pattern** using a `record` instead of a traditional C# `enum`. Its primary purpose is to create a strongly-typed, extensible, and descriptive vocabulary for the outcomes of your API operations, going beyond what standard HTTP status codes can offer.

While HTTP status codes (like `200`, `404`, `500`) tell a client what happened at the transport level, the `ResponseType` record provides more specific, semantic context about the business-level outcome within the JSON response body.

For example, an HTTP `400 Bad Request` is a generic client error. The `ResponseType` can clarify the cause:

* **ResponseType.Validation**: The request failed because one or more input fields were invalid.
* **ResponseType.BadRequest**: The request was malformed or syntactically incorrect in a way that goes beyond simple validation.

The primary purpose is to create a consistent contract for your APIs. A client application can parse the `responseType` field in the JSON payload to trigger specific logic (e.g., display validation errors on the correct form fields) without having to guess the meaning of a generic HTTP status code.

### Why a record instead of an enum?

Traditional C# enums cannot be extended from consuming projects. By using a `record`, `ResponseType` provides the same strong-typing benefits of an enum while allowing consuming projects to define their own response types via inheritance.

The `[JsonConverter(typeof(ResponseTypeJsonConverter))]` attribute ensures that the record is serialized to JSON as a plain string (e.g., `"NotFound"`) rather than as a complex object, making the API response self-documenting and easy for developers to read.

## Built-in Values

| Member                | Description                                                                     | Typical HTTP Status |
|-----------------------|---------------------------------------------------------------------------------|---------------------|
| **Success**           | The operation was successful.                                                   | 200 OK              |
| **Created**           | A resource was successfully created.                                            | 201 Created         |
| **Updated**           | A resource was successfully updated.                                            | 200 OK              |
| **Deleted**           | A resource was successfully deleted.                                            | 200 OK              |
| **NoContent**         | The request succeeded with no content to return.                                | 204 No Content      |
| **Accepted**          | The request was accepted for processing but is not yet complete.                | 202 Accepted        |
| **Validation**        | The request failed due to invalid input data.                                   | 400 Bad Request     |
| **Problem**           | An unexpected problem occurred, often used for RFC 7807 problem details.        | 500 Internal Server |
| **NotFound**          | The requested resource was not found.                                           | 404 Not Found       |
| **Conflict**          | The request conflicts with the current state of the resource.                   | 409 Conflict        |
| **Unauthorized**      | The request requires user authentication.                                       | 401 Unauthorized    |
| **Error**             | A general, non-specific error occurred during the operation.                    | 500 Internal Server |
| **BadRequest**        | The request was malformed or could not be processed for reasons other than validation. | 400 Bad Request     |
| **Unprocessable**     | The request was semantically incorrect and could not be processed.              | 422 Unprocessable   |
| **Forbidden**         | The server understood the request but refuses to authorize it.                  | 403 Forbidden       |
| **TooManyRequests**   | The client has sent too many requests in a given amount of time.                | 429 Too Many Requests |
| **ServiceUnavailable**| The service is temporarily unavailable.                                         | 503 Service Unavailable |
| **GatewayTimeout**    | The server, acting as a gateway, did not receive a timely response.             | 504 Gateway Timeout |

## Extending ResponseType

Since enums can't be extended, `ResponseType` is a `record` designed for inheritance. Consuming projects can define their own response types while maintaining the same strongly-typed API.

### Step 1: Define a custom response type

Create a `record` that inherits from `ResponseType`. Use a private constructor and expose a static `Instance` field:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;

namespace MyApp.ResponseTypes;

/// <summary>
/// Represents an HTTP 402 Payment Required response type.
/// </summary>
public record PaymentRequiredResponseType : ResponseType
{
    private PaymentRequiredResponseType(string value) : base(value) { }

    public static readonly PaymentRequiredResponseType Instance = new("PaymentRequired");
}
```

### Step 2: Define a custom exception

Create an exception that uses your custom response type:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace MyApp.Exceptions;

public class PaymentRequiredException : RaBaseException
{
    public PaymentRequiredException(string message = "Payment is required.")
        : base(402, PaymentRequiredResponseType.Instance, message)
    {
    }
}
```

### Step 3: Throw and handle the exception

Throw it like any built-in exception, and the exception-to-response mapper will use your custom response type:

```csharp showLineNumbers
// Throw
throw new PaymentRequiredException("Premium subscription required.");

// Or throw a generic RaBaseException with the custom type directly
throw new RaBaseException(402, PaymentRequiredResponseType.Instance, "Premium subscription required.");
```

### Step 4: JSON output

The custom response type serializes as a plain string in the API response:

```json showLineNumbers
{
  "responseCode": 402,
  "responseType": "PaymentRequired",
  "responseMessage": "Premium subscription required."
}
```

### Grouping related types

For multiple custom types, group them in a single static class for discoverability:

```csharp showLineNumbers
using RA.Utilities.Core.Constants;

namespace MyApp.ResponseTypes;

public static class AppResponseTypes
{
    public static readonly PaymentRequiredResponseType PaymentRequired = PaymentRequiredResponseType.Instance;
    public static readonly RateLimitedResponseType RateLimited = RateLimitedResponseType.Instance;
    public static readonly ServiceUnavailableResponseType ServiceUnavailable = ServiceUnavailableResponseType.Instance;
}

// Usage
throw new RaBaseException(429, AppResponseTypes.RateLimited, "Too many requests.");
```

### Comparison with built-in types

Custom types interoperate seamlessly with built-in ones — assignability, equality, and serialization all work:

```csharp showLineNumbers
ResponseType a = ResponseType.NotFound;                 // Built-in
ResponseType b = PaymentRequiredResponseType.Instance;  // Custom

bool same = a == b; // false — equality is based on the Value string

// All serialize the same way in JSON:
// "NotFound"
// "PaymentRequired"
```

## Example JSON Response

```json showLineNumbers
{
  "responseCode": 404,
  // highlight-next-line
  "responseType": "NotFound", // From the ResponseType record
  "responseMessage": "Product with value '99' not found."
}
```
