---
title: Response<T>
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `Response<T>` class is the universal wrapper for all API responses within the RA.Utilities ecosystem. It provides a standardized structure for returning data, errors, and status information from your API endpoints, ensuring consistency and predictability for clients.

### 🎯 Purpose

The `Response<T>` class is the foundational building block for creating standardized and predictable API responses in your application.
Its primary purpose is to act as a universal wrapper for every response that your API sends back to a client.

By using this single, consistent structure, you gain several key advantages:

1. **Standardization**: Every response, whether it's a success carrying data, a validation error, or a server-side failure, will have the same shape.
It will always contain a `ResponseCode`, `ResponseType`, `ResponseMessage`, and a `Result` payload.

2. **Predictability for Clients**: Frontend applications and other API consumers benefit immensely from this consistency.
They can write generic handling logic to parse the response wrapper before dealing with the specific data inside the `Result` property.
This simplifies client-side code and makes it more robust.

3. **Clear Communication**: The wrapper explicitly communicates the outcome of the API call.
The ResponseType enum (e.g., `Success`, `BadRequest`, `NotFound`) and the ResponseMessage provide immediate, clear context about what happened.

4. **Extensibility**: It serves as the base class for more specific, semantic response types like `SuccessResponse<T>`, `BadRequestResponse`, and `NotFoundResponse`.
These derived classes pre-configure the `Response<T>` properties for common scenarios, reducing boilerplate code in your controllers.

In short, `Response<T>` is the cornerstone of a strategy to make your API's communication clear, consistent, and easy to work with for any consumer.

### Class Definition

```csharp showLineNumbers
public class Response<T>
{
    public int ResponseCode { get; set; }
    public ResponseType ResponseType { get; set; }
    public string? ResponseMessage { get; set; }
    public T? Result { get; set; }
}
```

### Properties

| Property          | Type                                       | Description                                                              |
| ----------------- | ------------------------------------------ | ------------------------------------------------------------------------ |
| **ResponseCode**    | `int`                                      | A code for the response, often mapping to an HTTP status code.           |
| **ResponseType**    | `RA.Utilities.Core.Constants.ResponseType` | An enum indicating the type of response (e.g., `Success`, `Error`, `NotFound`). |
| **ResponseMessage** | `string?`                                  | A human-friendly message describing the outcome of the operation.        |
| **Result**          | `T?`                                       | The actual data payload of the response. Can be `null` for error responses. |

### Generic JSON Example

The following example shows the structure of a successful response containing a `Product` object.
The `result` property would be `null` or contain error details for other response types.

```json showLineNumbers
{
  "responseCode": 200,
  "responseType": "Success",
  "responseMessage": "Operation completed successfully.",
  "result": {
    "id": 123,
    "name": "Example Product"
  }
}
```
