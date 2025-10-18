using System;
using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Api.Mapper;

/// <summary>
/// Provides custom result helpers for API endpoints.
/// </summary>
public static class ErrorResultResponse
{
    /// <summary>
    /// Creates an <see cref="IResult"/> for problem details based on an <see cref="Exception"/> object.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> object containing the error information.</param>
    /// <returns>An <see cref="IResult"/> representing the problem details.</returns>
    public static IResult Result(Exception exception) => exception switch
    {
        BadRequestException badRequestException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToBadRequestResponse(badRequestException),
            statusCode: BaseResponseCode.BadRequest
        ),
        ConflictException conflictException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.MapToConflictResponse(conflictException),
            statusCode: BaseResponseCode.Conflict
        ),
        NotFoundException notFoundException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.MapToNotFoundResponse(notFoundException),
            statusCode: BaseResponseCode.NotFound
        ),
        RaBaseException baseException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToGeneralErrorResponse(baseException),
            statusCode: BaseResponseCode.InternalServerError
        ),
        _ => Microsoft.AspNetCore.Http.Results.Json(new ErrorResponse(), statusCode: BaseResponseCode.InternalServerError)
    };
}
