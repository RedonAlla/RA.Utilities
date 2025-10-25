---
title: BadRequestResponse
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `BadRequestResponse` class is a specialized model for creating standardized `400 Bad Request` responses, typically used to communicate validation failures to the client. It inherits from [`Response<T>`](./Response), with the `Result` property typed as an array of `BadRequestResult` objects.

### 🎯 Purpose

The `BadRequestResponse` class is a specialized model designed to create standardized `400 Bad Request `API responses, typically used for validation failures.
It inherits from the base [`Response<T>`](./Response) class and streamlines the process of communicating input errors to the client.

Here's why it's so useful:

1. **Standardizes Validation Errors**: It provides a consistent structure for all validation-related errors.
The `Result` property is specifically typed to hold an array of `BadRequestResult` objects, where each object details a single validation failure (e.g., which property failed, the error message, and the value that was attempted).

2. **Reduces Boilerplate**: It automatically sets the response properties for a bad request:

  * **`ResponseCode`**: Set to `400` (from `BaseResponseCode.BadRequest`).
  * **`ResponseType`**: Set to `ResponseType.BadRequest`.
  * **ResponseMessage**: Defaults to `"One or more validation errors occurred."`.

3. **Provides Clear Client Feedback**: By returning a structured list of errors, you give the client precise, actionable feedback.
A frontend application can easily parse this list and display the appropriate error message next to each invalid input field.

Using `BadRequestResponse` ensures that your API communicates validation problems clearly and consistently, which is essential for a good developer experience.

### ⚙️ How It Works

When you create an instance of `BadRequestResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `400` (from `BaseResponseCode.BadRequest`).
- **`ResponseType`**: Set to `ResponseType.BadRequest`.
- **`ResponseMessage`**: Defaults to `"One or more validation errors occurred."`.
- **`Result`**: Contains an array of `BadRequestResult` objects, each detailing a specific validation error.

### 🚀 Usage in a Controller

You can use this class in your controller actions when input validation fails. Create one or more `BadRequestResult` objects and pass them to the `BadRequestResponse` constructor.

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
            var validationError = new BadRequestResult(
                propertyName: "Email",
                errorMessage: "'Email' must not be empty.",
                attemptedValue: request.Email,
                errorCode: "NotEmptyValidator"
            );

            // Return a 400 Bad Request with the structured error
            // highlight-next-line
            return BadRequest(new BadRequestResponse(new[] { validationError }));
        }

        // ... proceed with user creation
        return Ok();
    }
}
```

### Example JSON Output

The code above would produce the following JSON response body if the email was empty:

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
      "errorCode": "NotEmptyValidator"
    }
  ]
}
```
