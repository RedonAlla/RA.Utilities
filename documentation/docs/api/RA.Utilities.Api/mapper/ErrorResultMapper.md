```bash
Namespace: RA.Utilities.Api.Mapper
```

The `ErrorResultMapper` is a static helper class responsible for the low-level task of **transforming** specific exception types from the [`RA.Utilities.Core.Exceptions`](../../../core/RA.Utilities.Core.Exceptions/index.mdx) package into their corresponding standardized response models from [`RA.Utilities.Api.Results`](../../ApiResults/index.mdx).

## 🎯 Purpose

The primary purpose of `ErrorResultMapper` is to centralize the logic for converting exception data into structured DTOs (Data Transfer Objects).
For example, it takes a [`NotFoundException`](../../../core/RA.Utilities.Core.Exceptions/NotFoundException.md) and creates a
[`NotFoundResponse`](../../ApiResults/NotFoundResponse.md) object, correctly populating the `EntityName` and `EntityValue` fields.

This class acts as an internal "factory" for error response bodies.
It is consumed by other helpers, most notably the [`ErrorResultResponse`](./ErrorResultResponse.md) class, to ensure that all error responses are created consistently.

## 🔗 Relationship with [`ErrorResultResponse`](./ErrorResultResponse.md)

While both classes are involved in error handling, they have distinct roles:

- **`ErrorResultMapper`**: Creates the **body** of the error response (e.g., a `NotFoundResponse` object).
It deals with the "what" of the error.
- **`ErrorResultResponse`**: Creates the final **`IResult`** for the API endpoint.
It takes the response body from `ErrorResultMapper` and wraps it in an `IResult` (like `Results.Json`) with the correct HTTP status code. It deals with the "how" of returning the error.

In general, **you will not interact with `ErrorResultMapper` directly**. You will use [`ErrorResultResponse`](./ErrorResultResponse.md), which uses this mapper internally.

## 🚀 How It's Used (Indirectly)

The `ErrorResultResponse.Result` method is a `switch` expression that calls the appropriate `ErrorResultMapper` method based on the exception type.

Here is a simplified view of the code from `ErrorResultResponse.cs` to illustrate the relationship:

```csharp showLineNumbers
// From RA.Utilities.Api.Mapper.ErrorResultResponse
public static IResult Result(Exception exception) => exception switch
{
    // ... other cases

    NotFoundException notFoundException => Microsoft.AspNetCore.Http.Results.Json(
        // highlight-next-line
        ErrorResultMapper.MapToNotFoundResponse(notFoundException), // Creates the response body
        statusCode: 404
    ),

    // ... other cases
};
```

As you can see, [`ErrorResultResponse`](./ErrorResultResponse.md) delegates the creation of the response body to `ErrorResultMapper` before creating the final `Json` result.

## 📦 Available Mappers

The class provides a dedicated mapping method for each custom exception type:

- **`ToBadRequestResponse(BadRequestException ex)`**:
  Maps a `BadRequestException` to a `BadRequestResponse`, converting the exception's validation errors into a `BadRequestResult` array.

- **`MapToConflictResponse(ConflictException ex)`**:
  Maps a `ConflictException` to a `ConflictResponse`, populating the `ConflictResult` with the entity name and value.

- **`MapToUnauthorizedResponse(UnauthorizedException ex)`**:
  Maps a `UnauthorizedException` to a `UnauthorizedResponse`, populating the `UnauthorizedResult` and generating a descriptive message.

- **`ToGeneralErrorResponse(RaBaseException ex)`**:
  Maps any other `RaBaseException` to a generic `ErrorResponse`, using the exception's message and error code.

- **`ToGeneralErrorResponse(RaBaseException ex)`**:
  Maps any other `RaBaseException` to a generic `ErrorResponse`, using the exception's message and error code.

