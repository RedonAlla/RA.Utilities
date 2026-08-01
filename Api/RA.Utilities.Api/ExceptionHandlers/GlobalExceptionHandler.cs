using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RA.Utilities.Api.Mapper;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Api.ExceptionHandlers;

/// <summary>
/// A global exception handler for ASP.NET Core applications that implements <see cref="IExceptionHandler"/>.
/// This handler acts as a centralized safety net to catch any unhandled exceptions that occur during request processing.
/// It logs the exception and transforms it into a standardized, structured JSON error response using the <see cref="ErrorResultResponse"/> mapper.
/// </summary>
/// <remarks>
/// This handler is registered with the dependency injection container and the ASP.NET Core pipeline
/// via the <c>AddRaExceptionHandling()</c> and <c>UseRaExceptionHandling()</c> extension methods.
/// It ensures that the API always returns a consistent error format to the client,
/// converting semantic exceptions (like <c>NotFoundException</c>) into appropriate HTTP status codes.
/// </remarks>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Asynchronously handles an unhandled exception by logging it and writing a standardized JSON error response to the <see cref="HttpResponse"/>.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
    /// <param name="exception">The unhandled exception to be handled.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> for cancelling the operation.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation, containing <c>true</c> to indicate that the exception has been handled.
    /// This implementation always returns <c>true</c> as it considers all exceptions processed by it to be handled.
    /// </returns>
    /// <remarks>
    /// This method performs the following steps:
    /// 1. Logs the exception details using the injected <see cref="ILogger"/>.
    /// 2. Maps the <paramref name="exception"/> to a standard <see cref="IResult"/> using <see cref="ErrorResultResponse.Result(Exception)"/>.
    /// 3. Executes the resulting <see cref="IResult"/>, which writes the JSON error response to the client.
    /// </remarks>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        LogException(exception);

        IResult problemResult = ErrorResultResponse.Result(exception);
        await problemResult.ExecuteAsync(httpContext);

        return true;
    }

    /// <summary>
    /// Logs the exception at the appropriate level. Client errors (4xx) are logged as warnings;
    /// server errors (5xx) and unrecognized exceptions are logged as errors.
    /// </summary>
    private void LogException(Exception exception)
    {
        if (IsClientError(exception))
        {
            logger.LogWarning(exception, "A client error occurred: {Message}", exception.Message);
        }
        else
        {
            logger.LogError(exception, "An exception has occurred: {Message}", exception.Message);
        }
    }

    private static bool IsClientError(Exception exception) => exception switch
    {
        BadRequestException
            or ConflictException
            or UnprocessableException
            or NotFoundException
            or UnauthorizedException
            or ForbiddenException => true,
        _ => false,
    };
}
