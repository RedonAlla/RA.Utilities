---
title: UnauthorizedResponse
---

```bash
Namespace: RA.Utilities.Api.Results
```

The `UnauthorizedResponse` class is a specialized Data Transfer Object (DTO) designed to create a standardized and consistent JSON response body whenever your API needs to return an HTTP 401 Unauthorized error.

Its main goals are:

#### 1. Standardization:
It ensures that every 401 error response from your API has the exact same structure.
It inherits from the base `Response<T>` class, so it includes common fields like `ResponseCode`, `ResponseMessage`, and `ResponseType`.

#### 2. Semantic Meaning:
It sets the ResponseType property to `ResponseType.Unauthorized`.
As shown in your documentation, this provides a clear, machine-readable signal in the JSON payload that goes beyond the HTTP status code, telling the client exactly what kind of error occurred.

#### 3. Simplicity:
It encapsulates the logic for creating a 401 response body, so your error handling code doesn't need to build it manually.


## 🛠️ How It's Used in Your Application
The class is a key part of your API's exception handling pipeline:

1. Some part of your application throws an `UnauthorizedException`.
2. The `GlobalExceptionHandler` (not shown, but implied by the structure) catches this exception.
3. It calls `ErrorResultResponse.Result(exception)`.
4. The switch expression in `ErrorResultResponse` matches the `UnauthorizedException` and calls `ErrorResultMapper.MapToUnauthorizedResponse(exception)`.
5. This mapper creates an instance of your `UnauthorizedResponse` class, populating it with the correct code (401) and message.
6. Finally, this `UnauthorizedResponse` object is serialized into a JSON string and sent to the client with an HTTP 401 status code.

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 401,
  "responseType": "Unauthorized",
  "responseMessage": "User is not authorized to perform this action.",
  "result": null
}
```