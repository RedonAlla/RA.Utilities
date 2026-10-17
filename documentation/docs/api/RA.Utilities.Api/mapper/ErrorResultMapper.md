```bash
Namespace: RA.Utilities.Api.Mapper
```

The `ErrorResultMapper` is a static helper class responsible for the low-level task of **transforming** specific exception types from the [`RA.Utilities.Core.Exceptions`](../../../core/RA.Utilities.Core.Exceptions/index.mdx) package into their corresponding standardized response models from `RA.Utilities.Api` (namespace `RA.Utilities.Api.Response`).

## 🎯 Purpose

The primary purpose of `ErrorResultMapper` is to centralize the logic for converting exception data into structured DTOs (Data Transfer Objects).
For example, it takes a [`NotFoundException`](../../../core/RA.Utilities.Core.Exceptions/NotFoundException.md) and creates a
[`NotFoundResponse`](../../RA.Utilities.Api.Response/NotFoundResponse.md) object, correctly populating the `Entity` and `Value` fields.

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
        ErrorResultMapper.ToResponse(notFoundException), // Creates the response body
        statusCode: BaseResponseCode.NotFound
    ),

    // ... other cases
};
```

As you can see, [`ErrorResultResponse`](./ErrorResultResponse.md) delegates the creation of the response body to `ErrorResultMapper` before creating the final `Json` result.

## 📦 Available Mappers

The class provides a dedicated mapping method for each custom exception type:

- **`ToResponse(BadRequestException ex)`**:
  Maps a `BadRequestException` to a `BadRequestResponse`, converting the exception's validation errors into a `BadRequestResult` array.

- **`ToResponse(ConflictException ex)`**:
  Maps a `ConflictException` to a `ConflictResponse`, populating the `ConflictResult` with the entity name and value.

- **`ToResponse(NotFoundException ex)`**:
  Maps a `NotFoundException` to a `NotFoundResponse`, populating the `NotFoundResult` with the entity name and value.

- **`ToResponse(UnauthorizedException ex)`**:
  Maps an `UnauthorizedException` to an `UnauthorizedResponse`, populating the `ErrorResult` with the error code and message.

- **`ToResponse(ForbiddenException ex)`**:
  Maps a `ForbiddenException` to a `ForbiddenResponse`, populating the `ErrorResult` with the error code and message.

- **`ToResponse(UnprocessableException ex)`**:
  Maps an `UnprocessableException` to an `UnprocessableResponse`, populating the `ErrorResult` with the error code and message.

- **`ToResponse(TooManyRequestsException ex)`**:
  Maps a `TooManyRequestsException` to a [`TooManyRequestsResponse`](../../RA.Utilities.Api.Response/TooManyRequestsResponse.md) (HTTP 429), populating the `ErrorResult` with the error code and message.

- **`ToResponse(ServiceUnavailableException ex)`**:
  Maps a `ServiceUnavailableException` to a [`ServiceUnavailableResponse`](../../RA.Utilities.Api.Response/ServiceUnavailableResponse.md) (HTTP 503), populating the `ErrorResult` with the error code and message.

- **`ToResponse(GatewayTimeoutException ex)`**:
  Maps a `GatewayTimeoutException` to a [`GatewayTimeoutResponse`](../../RA.Utilities.Api.Response/GatewayTimeoutResponse.md) (HTTP 504), populating the `ErrorResult` with the error code and message.

- **`ToResponse(RaBaseException ex)`**:
  Maps any other `RaBaseException` to a generic `ErrorResponse`, using the exception's message and error code.

## 📦 Response Payload Shapes

The mapper does not populate the same payload for every exception. The table below reflects the current implementation:

| Exception | Response | `Result` payload |
|---|---|---|
| `BadRequestException` | `BadRequestResponse` | `BadRequestResult[]` built from `exception.Errors` |
| `ConflictException` | `ConflictResponse` | `ConflictResult` with `Entity`, `Value`, and `ErrorMessage` |
| `NotFoundException` | `NotFoundResponse` | `NotFoundResult` with `Entity` and `Value` |
| `UnauthorizedException` | `UnauthorizedResponse` | `null` — the envelope carries the code and message |
| `ForbiddenException` | `ForbiddenResponse` | `null` — the envelope carries the code and message |
| `UnprocessableException` | `UnprocessableResponse` | `null` — the envelope carries the code and message |
| `TooManyRequestsException` | `TooManyRequestsResponse` | `null` — the envelope carries the code and message |
| `ServiceUnavailableException` | `ServiceUnavailableResponse` | `null` — the envelope carries the code and message |
| `GatewayTimeoutException` | `GatewayTimeoutResponse` | `null` — the envelope carries the code and message |
| `RaBaseException` | `ErrorResponse` | `null` — the envelope carries the code and message |

:::warning[Known `NotFoundException` payload issue]

`ToResponse(NotFoundException)` currently passes `exception.Message` as the third positional argument of `NotFoundResult`, but that parameter is `errorCode`, not the message. As a result:

- `NotFoundResult.Entity` and `Value` are correct.
- `NotFoundResult.ErrorCode` contains the exception message.
- `NotFoundResult.ErrorMessage` is `""`.

The outer `NotFoundResponse.ResponseMessage` is still correct. A fix is pending; see [Troubleshooting](../troubleshooting.mdx#why-is-the-nested-notfoundresult-payload-wrong).

:::
