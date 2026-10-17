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

### JSON serialization

The `[JsonConverter(typeof(ResponseTypeJsonConverter))]` attribute ensures that the enum is serialized to JSON as a plain string (e.g., `"NotFound"`) rather than as a number, making the API response self-documenting and easy for developers to read.

Deserialization behavior:

* Member names are matched **case-insensitively** (`"notfound"` → `ResponseType.NotFound`).
* `null`, empty, whitespace, or **unknown** strings fall back to `ResponseType.Error` instead of throwing.

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

Use the standard `System.Enum` APIs. Note that `Enum.TryParse` succeeds for **any** name (even undefined ones), so validate with `Enum.IsDefined` when the input comes from an untrusted source:

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

### Default value

`default(ResponseType)` (and uninitialized `ResponseType` fields) is `ResponseType.Success`, the first member (value `0`). Keep this in mind when declaring optional properties.

### No custom members

The set of values is closed — consuming projects cannot add new members. For an outcome that has no dedicated member (e.g., HTTP 402 Payment Required), use the closest built-in member and communicate the specifics through `ErrorCode`/`ResponseCode` and the message:

```csharp showLineNumbers
throw new RaBaseException(402, ResponseType.Error, "Premium subscription required.");
```

This serializes as:

```json showLineNumbers
{
  "responseCode": 402,
  "responseType": "Error",
  "responseMessage": "Premium subscription required."
}
```

## Example JSON Response

```json showLineNumbers
{
  "responseCode": 404,
  // highlight-next-line
  "responseType": "NotFound", // From the ResponseType enum
  "responseMessage": "Product with value '99' not found."
}
```
