using Microsoft.AspNetCore.Http;
using RA.Utilities.Api.Results;

namespace RA.Utilities.Api.Mapper;

/// <summary>
/// Provides static helper methods for creating standardized, successful API responses
/// wrapped in the <see cref="RA.Utilities.Api.Results.SuccessResponse{T}"/> model.
/// </summary>
public static class SuccessResponse
{
    /// <summary>
    /// Creates a new <see cref="IResult"/> object that represents a 200 OK response with a payload.
    /// </summary>
    /// <typeparam name="TResult">The type of the payload.</typeparam>
    /// <param name="result">The payload to include in the response body.</param>
    /// <returns>An <see cref="IResult"/> representing a 200 OK response with the specified payload.</returns>
    public static IResult Ok<TResult>(TResult result) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.Ok(new SuccessResponse<TResult>(result));
    }

    /// <summary>
    /// Creates a new <see cref="IResult"/> object that represents a 200 OK response with a payload.
    /// </summary>
    /// <returns>An <see cref="IResult"/> representing a 200 OK response.</returns>
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
    /// Creates an HTTP 201 Created response with the specified route name, route values, and result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="routeName">The name of the route to use for generating the URL.</param>
    /// <param name="routeValues">The route data to use for generating the URL.</param>
    /// <param name="results">The result object.</param>
    public static IResult CreatedAtRoute<TResult>(string routeName, object routeValues, TResult results) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.CreatedAtRoute(
            routeName: routeName,
            routeValues: routeValues,
            value: new SuccessResponse<TResult>(results)
        );
    }

    /// <summary>
    /// Creates an HTTP 201 Created response with the specified route name and route values.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="routeName">The name of the route to use for generating the URL.</param>
    /// <param name="routeValues">The route data to use for generating the URL.</param>
    public static IResult CreatedAtRoute<TResult>(string routeName, object routeValues) where TResult : new()
    {
        return Microsoft.AspNetCore.Http.Results.CreatedAtRoute(
            routeName: routeName,
            routeValues: routeValues
        );
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
