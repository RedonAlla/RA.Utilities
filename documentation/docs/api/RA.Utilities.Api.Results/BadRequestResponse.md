---
title: BadRequestResponse
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `BadRequestResponse` class is a specialized model for creating standardized `400 Bad Request` responses, typically used to communicate validation failures to the client. It inherits from [`Response<T>`](./Response), with the `Result` property typed as an array of [`BadRequestResult`](#badrequestresult) objects.

### 🎯 Purpose

The `BadRequestResponse` class is a specialized model designed to create standardized `400 Bad Request` API responses, typically used for validation failures.
It inherits from the base [`Response<T>`](./Response) class and streamlines the process of communicating input errors to the client.

Here's why it's so useful:

1. **Standardizes Validation Errors**: It provides a consistent structure for all validation-related errors.
The `Result` property is specifically typed to hold an array of `BadRequestResult` objects, where each object details a single validation failure.

2. **Reduces Boilerplate**: It automatically sets the response properties for a bad request:

  * **`ResponseCode`**: Set to `400` (from `BaseResponseCode.BadRequest`).
  * **`ResponseType`**: Set to `ResponseType.BadRequest`.
  * **ResponseMessage**: Defaults to `"One or more validation errors occurred."` (from `BaseResponseMessages.BadRequest`).

3. **Provides Clear Client Feedback**: By returning a structured list of errors, you give the client precise, actionable feedback.

### ⚙️ How It Works

When you create an instance of `BadRequestResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `400` (from `BaseResponseCode.BadRequest`).
- **`ResponseType`**: Set to `ResponseType.BadRequest`.
- **`ResponseMessage`**: Defaults to `"One or more validation errors occurred."`.
- **`Result`**: Contains an array of [`BadRequestResult`](#badrequestresult) objects, each detailing a specific validation error.

### 🚀 Usage in a Controller

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            // Create a validation error detail
            var validationError = new BadRequestResult
            {
                PropertyName = "Email",
                ErrorMessage = "'Email' must not be empty.",
                AttemptedValue = request.Email,
                ErrorCode = "NotEmptyValidator"
            };

            // Return a 400 Bad Request with the structured error
            // highlight-next-line
            return BadRequest(new BadRequestResponse(new[] { validationError }));
        }

        // ... proceed with user creation
        return Ok();
    }
}
```

### BadRequestResult
Inherits from [`ErrorResult`](./ErrorResult).

| Property | Type | Description |
| -------- | --- | ------------------------------------------------ |
| **PropertyName** | `string` | The name of the property that failed validation. |
| **ErrorMessage** | `string` | The error message (inherited from `ErrorResult`). |
| **AttemptedValue** | `object` | The value that was sent in the request and caused the failure. |
| **ExpectedValue** | `object` | The expected value or format for the property. |
| **ErrorCode** | `string` | The specific error code (inherited from `ErrorResult`). |

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 400,
  "responseType": "BadRequest",
  "responseMessage": "One or more validation errors occurred.",
  "result": [
    {
      "propertyName": "Email",
      "errorMessage": "'Email' must not be empty.",
      "attemptedValue": "",
      "errorCode": "NotEmptyValidator",
      "expectedValue": null
    }
  ]
}
```
