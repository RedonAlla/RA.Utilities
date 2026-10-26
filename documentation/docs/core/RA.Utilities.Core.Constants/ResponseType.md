---
title: ResponseType
---

```bash
Namespace: RA.Utilities.Core.Constants
```

The `ResponseType` is a C# `enum` that provides a strongly-typed, descriptive vocabulary for the outcomes of your API operations, going beyond what standard HTTP status codes can offer.

While HTTP status codes (like `200`, `404`, `500`) tell a client what happened at the transport level, the `ResponseType` enum provides more specific, semantic context about the business-level outcome within the JSON response body.

For example, an HTTP `400 Bad Request` is a generic client error. The `ResponseType` can clarify the cause:

* **ResponseType.Validation**: The request failed because one or more input fields were invalid.
* **ResponseType.BadRequest**: The request was malformed or syntactically incorrect in a way that goes beyond simple validation.

The primary purpose is to create a consistent contract for your APIs. A client application can parse the `responseType` field in the JSON payload to trigger specific logic (e.g., display validation errors on the correct form fields) without having to guess the meaning of a generic HTTP status code.

## Built-in Values

`ResponseType` is a standard enum; the numbers below are the underlying member values and are part of the contract.

| Member                | Value | Description                                                                     | Typical HTTP Status |
|-----------------------|-------|---------------------------------------------------------------------------------|---------------------|
| **Success**           | 0     | The operation was successful.                                                   | 200 OK              |
| **Created**           | 1     | A resource was successfully created.                                            | 201 Created         |
| **Updated**           | 2     | A resource was successfully updated.                                            | 200 OK              |
| **Deleted**           | 3     | A resource was successfully deleted.                                            | 200 OK              |
| **NoContent**         | 4     | The request succeeded with no content to return.                                | 204 No Content      |
| **Accepted**          | 5     | The request was accepted for processing but is not yet complete.                | 202 Accepted        |
| **Validation**        | 6     | The request failed due to invalid input data.                                   | 400 Bad Request     |
| **Problem**           | 7     | An unexpected problem occurred, often used for RFC 7807 problem details.        | 500 Internal Server |
| **NotFound**          | 8     | The requested resource was not found.                                           | 404 Not Found       |
| **Conflict**          | 9     | The request conflicts with the current state of the resource.                   | 409 Conflict        |
| **Unauthorized**      | 10    | The request requires user authentication.                                       | 401 Unauthorized    |
| **Error**             | 11    | A general, non-specific error occurred during the operation.                    | 500 Internal Server |
| **BadRequest**        | 12    | The request was malformed or could not be processed for reasons other than validation. | 400 Bad Request     |
| **Unprocessable**     | 13    | The request was semantically incorrect and could not be processed.              | 422 Unprocessable   |
| **Forbidden**         | 14    | The server understood the request but refuses to authorize it.                  | 403 Forbidden       |
| **TooManyRequests**   | 15    | The client has sent too many requests in a given amount of time.                | 429 Too Many Requests |
| **ServiceUnavailable** | 16   | The service is temporarily unavailable.                                         | 503 Service Unavailable |
| **GatewayTimeout**    | 17    | The server, acting as a gateway, did not receive a timely response.             | 504 Gateway Timeout |
| **PaymentRequired**   | 18    | Payment is required to access the resource.                                     | 402 Payment Required |

`PaymentRequired` (value `18`) was added in v10.1.1; earlier versions have no dedicated member, so they use `Error` with a `402` error code.

## JSON serialization

`ResponseType` is a plain enum with **no built-in converter**. With `System.Text.Json`'s default options it is written as its **numeric value** and read back from a number:

```csharp showLineNumbers
// Default System.Text.Json behavior
string json = JsonSerializer.Serialize(ResponseType.NotFound);
// json == "8"

ResponseType type = JsonSerializer.Deserialize<ResponseType>("8");
// type == ResponseType.NotFound
```

If your API contract exposes names instead of numbers, register `JsonStringEnumConverter` on the serializer used by your endpoints:

```csharp showLineNumbers
using System.Text.Json.Serialization;

// Minimal APIs and Results.Json (what RA.Utilities.Api uses)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // highlight-next-line
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// MVC controllers use the same converter through a different options object
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // highlight-next-line
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```

With the converter registered:

* Member names are written as-is (`"NotFound"`). Use `new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)` for `"notFound"`.
* Names are matched case-insensitively on read (`"notfound"` → `ResponseType.NotFound`).
* Unknown names and `null` for a non-nullable enum throw `JsonException`.
* Integer values are still accepted by default.

```json showLineNumbers
{
  "responseCode": 404,
  // highlight-next-line
  "responseType": "NotFound", // With JsonStringEnumConverter; otherwise 8
  "responseMessage": "Product with value '99' not found."
}
```

> **Changed in v10.0.4:** Before v10.0.4 `ResponseType` was a `record` with a built-in `ResponseTypeJsonConverter` that always wrote strings. Now that it is an enum, string output requires the opt-in above. See the [migration guide](./migration-guides.mdx).

## Working with the enum

### Switch expressions

Because `ResponseType` is a standard `enum`, it works with constant patterns and the compiler can verify exhaustiveness:

```csharp showLineNumbers
var label = responseType switch
{
    ResponseType.Success => "OK",
    ResponseType.NotFound => "Not Found",
    ResponseType.Conflict => "Conflict",
    ResponseType.BadRequest => "Bad Request",
    _ => "Other"
};
```

### Parsing from strings

Use the standard `System.Enum` APIs. Note that `Enum.TryParse` succeeds for **any** name (even undefined ones) and for numeric strings, so validate with `Enum.IsDefined` when the input comes from an untrusted source:

```csharp showLineNumbers
// ✅ Preferred — parse a known member name
if (Enum.TryParse("NotFound", ignoreCase: true, out ResponseType type))
{
    // type == ResponseType.NotFound
}

// ⚠️ Numeric strings parse successfully to *undefined* values
Enum.TryParse("123", out ResponseType undefined); // true — (ResponseType)123, not a defined member

// ✅ Validate untrusted input with Enum.IsDefined
if (Enum.TryParse(value, ignoreCase: true, out ResponseType parsed) && Enum.IsDefined(parsed))
{
    // safe to use parsed
}
```

### String representation

`ToString()` returns the member name:

```csharp showLineNumbers
string name = ResponseType.NotFound.ToString(); // "NotFound"
```

There is no implicit conversion between `ResponseType` and `string`.

### Default value

`default(ResponseType)` (and uninitialized `ResponseType` fields) is `ResponseType.Success`, the first member (value `0`). Keep this in mind when declaring optional properties.

### No custom members

The set of values is closed — consuming projects cannot add new members. For an outcome that has no dedicated member (e.g., HTTP 502 Bad Gateway), use the closest built-in member and communicate the specifics through `ErrorCode`/`ResponseCode` and the message:

```csharp showLineNumbers
throw new RaBaseException(502, ResponseType.Error, "The upstream service failed.");
```

This serializes as (with `JsonStringEnumConverter` configured):

```json showLineNumbers
{
  "responseCode": 502,
  "responseType": "Error",
  "responseMessage": "The upstream service failed."
}
```
