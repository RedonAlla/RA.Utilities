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
            responseCode: exception.ErrorCode,
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

        var result = new ConflictResult
        (
            exception.EntityName ?? string.Empty,
            exception.EntityValue?.ToString() ?? string.Empty
        );

        return new ConflictResponse(
            result
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

        var result = new NotFoundResult
        (
            exception.EntityName ?? string.Empty,
            exception.EntityValue?.ToString() ?? string.Empty
        );

        return new NotFoundResponse(
            result,
            responseMessage: $"{exception.EntityName} with value: '{exception.EntityValue}' not found."
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
            responseCode: exception.ErrorCode,
            responseMessage: exception.Message
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
            responseCode: exception.ErrorCode,
            responseMessage: exception.Message
        );
    }
}
