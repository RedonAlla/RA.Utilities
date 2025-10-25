using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Results;

namespace RA.Utilities.Api.Mapper;

/// <summary>
/// Provides static methods for creating API results.
/// </summary>
public static class SuccessResponse
{
    /// <summary>
    /// Creates an HTTP 200 OK response with the specified result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="results">The result object.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP 200 OK response.</returns>
    public static IResult Ok<TResult>(TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Ok(new SuccessResponse<TResult>(results));
    }

    /// <summary>
    /// Creates an HTTP 200 OK response without a specific result body.
    /// </summary>
    /// <returns>An <see cref="IResult"/> representing the HTTP 200 OK response.</returns>
    public static IResult Ok()
    {
        return Microsoft.AspNetCore.Http.Results.Ok();
    }

    /// <summary>
    /// Creates an HTTP 201 Created response.
    /// </summary>
    public static IResult Created()
    {
        return Microsoft.AspNetCore.Http.Results.Created();
    }

    /// <summary>
    /// Creates an HTTP 201 Created response with the specified result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="results">The result object.</param>
    public static IResult Created<TResult>(TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Created(string.Empty, new SuccessResponse<TResult>(results));
    }

    /// <summary>
    /// Creates an HTTP 201 Created response with the specified URI and result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="route">The URI at which the content has been created.</param>
    /// <param name="results">The result object.</param>
    public static IResult Created<TResult>(string route, TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Created(route, new SuccessResponse<TResult>(results));
    }

    /// <summary>
    /// Creates an HTTP 202 Accepted response.
    /// </summary>
    public static IResult Accepted()
    {
        return Microsoft.AspNetCore.Http.Results.Accepted();
    }

    /// <summary>
    /// Creates an HTTP 202 Accepted response with the specified URI.
    /// </summary>
    /// <param name="route">The URI with the location at which the status of requested content can be monitored.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP 202 Accepted response.</returns>
    public static IResult Accepted(string route)
    {
        return Microsoft.AspNetCore.Http.Results.Accepted(route);
    }

    /// <summary>
    /// Creates an HTTP 202 Accepted response with the specified result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="results">The result object.</param>
    public static IResult Accepted<TResult>(TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Accepted(string.Empty, new SuccessResponse<TResult>(results));
    }

    /// <summary>
    /// Creates an HTTP 202 Accepted response with the specified URI and result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="route">The URI with the location at which the status of requested content can be monitored.</param>
    /// <param name="results">The result object.</param>
    /// <returns>An <see cref="IResult"/> representing the HTTP 202 Accepted response.</returns>
    public static IResult Accepted<TResult>(string route, TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Accepted(route, new SuccessResponse<TResult>(results));
    }

    /// <summary>
    /// Creates an HTTP 204 No Content response.
    /// </summary>
    public static IResult NoContent()
    {
        return Microsoft.AspNetCore.Http.Results.NoContent();
    }
}
