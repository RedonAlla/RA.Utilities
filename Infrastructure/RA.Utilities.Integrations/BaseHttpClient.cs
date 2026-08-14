using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Utilities;

namespace RA.Utilities.Integrations;

/// <summary>
/// Contains common operations for making HTTP requests to external APIs.
/// This class is designed to be used with dependency injection and a configured <see cref="HttpClient"/>.
/// </summary>
public class BaseHttpClient
{
    /// <summary>
    /// The HTTP client.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for making requests.</param>
    /// <param name="settings">The integration settings, providing configuration like the base URL and timeout.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpClient"/> or <paramref name="settings"/> is null.</exception>
    public BaseHttpClient(HttpClient httpClient, IOptions<IIntegrationSettings> settings)
    {
        IIntegrationSettings settingsValue = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = settingsValue.BaseUrl;
        _httpClient.Timeout = TimeSpan.FromSeconds(settingsValue.Timeout);
    }

    /// <summary>
    /// A private helper method to construct and send an HTTP request.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="HttpContent"/> from the response.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    private async Task<HttpContent> BaseHttpCall<TRequestBody>(
        string action,
        HttpMethod method,
        IQueryStringRequest? queryString,
        IHeaderRequest? headers = null,
        TRequestBody? body = null,
        CancellationToken cancellationToken = default) where TRequestBody : class
    {
        string requestUri = QueryUtilities.ToQueryString(action, queryString);
        using var httpRequest = new HttpRequestMessage(method, requestUri);

        if (headers is not null)
        {
            foreach (KeyValuePair<string, string> header in headers.ToHeaders())
            {
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (body is not null)
        {
            httpRequest.Content = JsonConverterUtilities.ToStringContent(body);
        }

        HttpResponseMessage response =
            await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return response.Content;
    }

    /// <summary>
    /// Performs an HTTP GET request with the given request parameters and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized response
    /// of type <typeparamref name="TResponse"/>, or <see langword="null"/> if the response body is empty.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<TResponse?> GetAsync<TResponse>(
        string action,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        string response = await GetAsync(action, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response);
    }

    /// <summary>
    /// Performs an HTTP GET request with the given request parameters and returns the response content as a string.
    /// </summary>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> GetAsync(
        string action,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        HttpContent response = await BaseHttpCall<object>(action, HttpMethod.Get, queryString, headers, null, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP DELETE request with the given request parameters and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized response
    /// of type <typeparamref name="TResponse"/>, or <see langword="null"/> if the response body is empty.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<TResponse?> DeleteAsync<TResponse>(
        string action,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        string response = await DeleteAsync(action, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response);
    }

    /// <summary>
    /// Performs an HTTP DELETE request with the given request parameters and returns the response content as a string.
    /// </summary>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> DeleteAsync(
        string action,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        HttpContent response = await BaseHttpCall<object>(action, HttpMethod.Delete, queryString, headers, null, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP POST request with the given request data and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content, serialized as JSON.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized response
    /// of type <typeparamref name="TResponse"/>, or <see langword="null"/> if the response body is empty.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<TResponse?> PostAsync<TRequestBody, TResponse>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
        where TRequestBody : class
    {
        string response = await PostAsync(action, body, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response);
    }

    /// <summary>
    /// Performs an HTTP POST request with the given request data and returns the response content as a string.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content, serialized as JSON.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> PostAsync<TRequestBody>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TRequestBody : class
    {
        HttpContent response = await BaseHttpCall(action, HttpMethod.Post, queryString, headers, body, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Performs an HTTP PUT request with the given request data and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content, serialized as JSON.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the deserialized response
    /// of type <typeparamref name="TResponse"/>, or <see langword="null"/> if the response body is empty.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<TResponse?> PutAsync<TRequestBody, TResponse>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
        where TRequestBody : class
    {
        string response = await PutAsync(action, body, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response);
    }

    /// <summary>
    /// Performs an HTTP PUT request with the given request data and returns the response content as a string.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content, serialized as JSON.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">An object containing header values to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> PutAsync<TRequestBody>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        IHeaderRequest? headers = null,
        CancellationToken cancellationToken = default
    )
        where TRequestBody : class
    {
        HttpContent response = await BaseHttpCall(action, HttpMethod.Put, queryString, headers, body, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }
}
