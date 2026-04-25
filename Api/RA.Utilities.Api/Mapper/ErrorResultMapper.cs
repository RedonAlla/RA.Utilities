using System;
using System.Linq;
using RA.Utilities.Api.Results;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Api.Mapper;

/// <summary>
/// Provides static methods for mapping various exception types to their corresponding API error responses.
/// </summary>
public static class ErrorResultMapper
{
    /// <summary>
    /// Maps a <see cref="BadRequestException"/> to a <see cref="BadRequestResult"/>.
    /// </summary>
    /// <param name="exception">The <see cref="BadRequestException"/> instance to map from.</param>
    /// <returns>A <see cref="BadRequestResult"/> representing the mapped exception.</returns>
    public static BadRequestResponse ToBadRequestResponse(BadRequestException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        BadRequestResult[] result = [.. exception.Errors.Select(f => new BadRequestResult
        {
            PropertyName = f.PropertyName,
            ErrorMessage = f.ErrorMessage,
            AttemptedValue = f.AttemptedValue,
            ErrorCode = f.ErrorCode,
            ExpectedValue = f.ExpectedValue
        })];

        return new BadRequestResponse(
            errors: result,
            responseCode: exception.ResponseCode,
            responseMessage: BaseResponseMessages.BadRequest
        );
    }

    /// <summary>
    /// Maps a <see cref="ConflictException"/> to a <see cref="ConflictResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="ConflictException"/> instance to map from.</param>
    /// <returns>A <see cref="ConflictResponse"/> representing the mapped exception.</returns>
    public static ConflictResponse MapToConflictResponse(ConflictException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new ConflictResponse(
            new ConflictResult
            (
                exception.EntityName,
                exception.EntityValue,
                exception.ErrorCode,
                exception.Message
            ),
            exception.ResponseCode
        );
    }

    /// <summary>
    /// Maps an <see cref="UnprocessableException"/> to an <see cref="UnprocessableResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="UnprocessableException"/> instance to map from.</param>
    /// <returns>An <see cref="UnprocessableResponse"/> representing the mapped exception.</returns>
    public static UnprocessableResponse MapToUnprocessableResponse(UnprocessableException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new UnprocessableResponse(
            new ErrorResult
            {
                ErrorCode = exception.ErrorCode,
                ErrorMessage = exception.Message
            },
            exception.ResponseCode
        );
    }

    /// <summary>
    /// Maps a <see cref="NotFoundException"/> to a <see cref="NotFoundResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="NotFoundException"/> instance to map from.</param>
    /// <returns>A <see cref="NotFoundResponse"/> representing the mapped exception.</returns>
    public static NotFoundResponse MapToNotFoundResponse(NotFoundException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new NotFoundResponse(
            new NotFoundResult
            (
                exception.EntityName,
                exception.EntityValue,
                exception.ErrorCode,
                exception.Message
            ),
            responseCode: exception.ResponseCode,
            responseMessage: BaseResponseMessages.NotFound
        );
    }

    /// <summary>
    /// Maps an <see cref="UnauthorizedException"/> to an <see cref="UnauthorizedResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="UnauthorizedException"/> instance to map from.</param>
    /// <returns>An <see cref="UnauthorizedResponse"/> representing the mapped exception.</returns>
    public static UnauthorizedResponse MapToUnauthorizedResponse(UnauthorizedException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new UnauthorizedResponse(
            new ErrorResult
            {
                ErrorCode = exception.ErrorCode,
                ErrorMessage = exception.Message
            },
            responseCode: exception.ResponseCode,
            responseMessage: BaseResponseMessages.Unauthorized
        );
    }

    /// <summary>
    /// Maps a <see cref="ForbiddenException"/> to a <see cref="ForbiddenResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="ForbiddenException"/> instance to map from.</param>
    /// <returns>A <see cref="ForbiddenResponse"/> representing the mapped exception.</returns>
    public static ForbiddenResponse MapToForbiddenResponse(ForbiddenException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new ForbiddenResponse(
            new ErrorResult
            {
                ErrorCode = exception.ErrorCode,
                ErrorMessage = exception.Message
            },
            responseCode: exception.ResponseCode,
            responseMessage: BaseResponseMessages.Unauthorized
        );
    }

    /// <summary>
    /// Maps a <see cref="RaBaseException"/> to an <see cref="ErrorResponse"/>.
    /// </summary>
    /// <param name="exception">The <see cref="RaBaseException"/> instance to map from.</param>
    /// <returns>An <see cref="ErrorResponse"/> representing the mapped exception.</returns>
    public static ErrorResponse ToGeneralErrorResponse(RaBaseException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new ErrorResponse(
            new ErrorResult
            {
                ErrorCode = exception.ErrorCode,
                ErrorMessage = exception.Message
            },
            responseCode: exception.ResponseCode
        );
    }
}
