using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RA.Utilities.Api.Mapper;

namespace RA.Utilities.Api.ExceptionHandlers;

/// <summary>
/// Global exception handler for the API.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred.");

        IResult problemResult = ErrorResultResponse.Result(exception);
        await problemResult.ExecuteAsync(httpContext);

        return true;
    }
}
