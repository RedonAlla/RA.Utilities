---
title: ErrorResponse
sidebar_position: 6
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ErrorResponse` class is a specialized model for creating standardized `500 Internal Server Error` responses.
It is designed to be a "catch-all" for any unexpected or unhandled exceptions that occur within your application.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an `ErrorResult` object.

### 🎯 Purpose

The `ErrorResponse` class is a specialized model for creating standardized `500 Internal Server Error` API responses.
It serves as a "catch-all" for any unexpected or unhandled exceptions that occur during request processing.
It inherits from the base [`Response<T>`](./Response) class.

Its primary functions are:

1. **Handles Unexpected Failures**: It provides a consistent and safe way to respond when something goes wrong on the server that wasn't anticipated by a more specific exception (like `NotFoundException` or `ConflictException`).

2. **Prevents Information Leaks**: By default, it returns a generic error message.
In a production environment, it avoids sending sensitive details like exception messages or stack traces to the client, which could be a security risk.

3. **Provides Rich Debugging Info**: In a development environment, it can be configured to include detailed information in its `ErrorResult` payload, such as the exception type, the full exception message, and the stack trace.
This is invaluable for developers during debugging.

4. **Reduces Boilerplate**: It automatically sets the response properties for a server error:

  * **ResponseCode**: Set to `500` (from `BaseResponseCode.InternalServerError`).
  * **ResponseType**: Set to `ResponseType.Error`.
  * **ResponseMessage**: Defaults to `"An unexpected error occurred on the server."`.

The `GlobalExceptionHandler` in the `RA.Utilities.Api` package is designed to automatically catch unhandled exceptions and generate this `ErrorResponse`, ensuring your API always returns a structured JSON response, even when things go wrong.

### ⚙️ How It Works

When you create an instance of `ErrorResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `500` (from `BaseResponseCode.InternalServerError`).
- **`ResponseType`**: Set to `ResponseType.Error`.
- **`ResponseMessage`**: Defaults to `"An unexpected error occurred on the server."`.
- **`Result`**: An `ErrorResult` object containing details about the exception.

### 🚀 Usage with `GlobalExceptionHandler`

The most common use case for `ErrorResponse` is in conjunction with a global exception handler. The `GlobalExceptionHandler` provided in the `RA.Utilities.Api` package automatically catches any unhandled exceptions and uses `ErrorResponse` to generate the final output.

You typically do not need to create an `ErrorResponse` manually.

```csharp showLineNumbers
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddRaExceptionHandling(); // Registers the handler

var app = builder.Build();

// highlight-next-line
app.UseRaExceptionHandling(); // Adds the handler to the pipeline

app.MapGet("/test-error", () =>
{
    // This will be caught by the GlobalExceptionHandler
    throw new InvalidOperationException("Something went wrong!");
});

app.Run();
```

### Example JSON Output

The response format depends on the environment.

#### Production Environment

In production, the stack trace is omitted to prevent leaking sensitive information.

```json showLineNumbers
{
  "responseCode": 500,
  "responseType": "Error",
  "responseMessage": "An unexpected error occurred on the server.",
  "result": {
    "exceptionType": "System.InvalidOperationException",
    "message": "An error occurred while processing your request.",
    "stackTrace": null
  }
}
```

#### Development Environment

In development, the full exception details are included for easier debugging.

```json showLineNumbers
{
  "responseCode": 500,
  "responseType": "Error",
  "responseMessage": "An unexpected error occurred on the server.",
  "result": {
    "exceptionType": "System.InvalidOperationException",
    "message": "Something went wrong!",
    "stackTrace": "at Program.<>c... in /path/to/Program.cs:line 15"
  }
}
```
