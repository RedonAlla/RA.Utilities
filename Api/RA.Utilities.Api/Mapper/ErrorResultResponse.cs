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
            data: ErrorResultMapper.ToBadRequestResponse(badRequestException),
            statusCode: badRequestException.ResponseCode
        ),
        ConflictException conflictException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.MapToConflictResponse(conflictException),
            statusCode: conflictException.ResponseCode
        ),
        UnprocessableException unprocessableException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.MapToUnprocessableResponse(unprocessableException),
            statusCode: unprocessableException.ResponseCode
        ),
        NotFoundException notFoundException => Microsoft.AspNetCore.Http.Results.Json(
            ErrorResultMapper.MapToNotFoundResponse(notFoundException),
            statusCode: notFoundException.ResponseCode
        ),
        UnauthorizedException baseException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.MapToUnauthorizedResponse(baseException),
            statusCode: baseException.ResponseCode
        ),
        ForbiddenException forbiddenException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.MapToForbiddenResponse(forbiddenException),
            statusCode: forbiddenException.ResponseCode
        ),
        RaBaseException baseException => Microsoft.AspNetCore.Http.Results.Json(
            data: ErrorResultMapper.ToGeneralErrorResponse(baseException),
            statusCode: baseException.ResponseCode
        ),
        _ => Microsoft.AspNetCore.Http.Results.Json(new ErrorResponse(), statusCode: BaseResponseCode.InternalServerError)
    };
}
