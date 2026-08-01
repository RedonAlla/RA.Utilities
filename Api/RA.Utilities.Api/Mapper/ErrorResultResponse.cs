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
    /// <remarks>
    /// This method acts as a central dispatcher, using pattern matching to map custom domain exceptions 
    /// to their corresponding standardized API response models.
    /// </remarks>
    /// <param name="exception">The <see cref="Exception"/> object containing the error information.</param>
    /// <returns>An <see cref="IResult"/> representing the problem details.</returns>
    public static IResult Result(Exception exception) => exception switch
    {
        BadRequestException badRequestException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToResponse(badRequestException),
            statusCode: StatusCodes.Status400BadRequest
        ),
        ConflictException conflictException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.ToResponse(conflictException),
            statusCode: StatusCodes.Status409Conflict
        ),
        UnprocessableException unprocessableException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.ToResponse(unprocessableException),
            statusCode: StatusCodes.Status422UnprocessableEntity
        ),
        NotFoundException notFoundException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.ToResponse(notFoundException),
            statusCode: StatusCodes.Status404NotFound
        ),
        UnauthorizedException baseException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToResponse(baseException),
            statusCode: StatusCodes.Status401Unauthorized
        ),
        ForbiddenException forbiddenException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToResponse(forbiddenException),
            statusCode: StatusCodes.Status403Forbidden
        ),
        RaBaseException baseException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToResponse(baseException),
            statusCode: StatusCodes.Status500InternalServerError
        ),
        _ => Microsoft.AspNetCore.Http.Results.Json(new ErrorResponse(), statusCode: StatusCodes.Status500InternalServerError)
    };
}
