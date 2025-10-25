using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Extensions;
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
    /// The integration settings.
    /// </summary>
    private readonly IIntegrationSettings _settings;

    /// <summary>
    /// Gets the character encoding from the integration settings.
    /// </summary>
    private Encoding Encoding => Encoding.GetEncoding(_settings.Encoding);

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for making requests.</param>
    /// <param name="settings">The integration settings, providing configuration like media type and encoding.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="httpClient"/> is null.</exception>
    public BaseHttpClient(HttpClient httpClient, IOptions<IIntegrationSettings> settings)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = _settings.BaseUrl;
        _httpClient.DefaultRequestHeaders.Add("Accept", _settings.MediaType);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Timeout);
    }

    /// <summary>
    /// Performs an HTTP GET request with the given request parameters and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse> GetAsync<TResponse>(
        string action,
        IQueryStringRequest? queryString = default,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        string response = await GetAsync(action, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response, _settings.MediaType)!;
    }

    /// <summary>
    /// Performs an HTTP GET request with the given request parameters and returns the response content as a string.
    /// </summary>
    /// <param name="action">Endpoint to call.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> GetAsync(
        string action,
        IQueryStringRequest? queryString = default,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        HttpContent response = await BaseHttpCall<object>(action, HttpMethod.Get, queryString, headers, null, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// A private helper method to construct and send an HTTP request.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="method">The HTTP method to use for the request.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="HttpContent"/> from the response.</returns>
    private async Task<HttpContent> BaseHttpCall<TRequestBody>(
        string action,
        HttpMethod method,
        IQueryStringRequest? queryString,
        Dictionary<string, string>? headers = null,
        TRequestBody? body = null,
        CancellationToken cancellationToken = default) where TRequestBody : class
    {
        string requestUri = queryString?.ToQueryString(action);
        using var httpRequest = new HttpRequestMessage(method, requestUri);

        if (headers is not null)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                httpRequest.Headers.AddSafe(header.Key, header.Value);
            }
        }

        if (body is not null)
        {
            httpRequest.Content = JsonConverterUtilities.ToStringContent(body, _settings.MediaType, Encoding);
        }

        HttpResponseMessage response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return response.Content;
    }

    /// <summary>
    /// Performs an HTTP POST request with the given request data and deserializes the response.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse> PostAsync<TRequestBody, TResponse>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
        where TRequestBody : class
    {
        string response = await PostAsync(action, body, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response, _settings.MediaType)!;
    }

    /// <summary>
    /// Performs an HTTP POST request with the given request data and returns the response content as a string.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> PostAsync<TRequestBody>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        Dictionary<string, string>? headers = null,
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
    /// <typeparam name="TResponse">The type to deserialize the JSON response into.</typeparam>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized response of type <typeparamref name="TResponse"/>.</returns>
    public async Task<TResponse> PutAsync<TRequestBody, TResponse>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
        where TRequestBody : class
    {
        string response = await PutAsync(action, body, queryString, headers, cancellationToken);
        return JsonConverterUtilities.ToObject<TResponse>(response, _settings.MediaType)!;
    }

    /// <summary>
    /// Performs an HTTP PUT request with the given request data and returns the response content as a string.
    /// </summary>
    /// <typeparam name="TRequestBody">The type of the request body.</typeparam>
    /// <param name="action">The target endpoint or action.</param>
    /// <param name="body">The request body content.</param>
    /// <param name="queryString">An object containing query string parameters.</param>
    /// <param name="headers">A dictionary of custom headers to add to the request.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response body as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response status code indicates an error.</exception>
    public async Task<string> PutAsync<TRequestBody>(
        string action,
        TRequestBody? body = null,
        IQueryStringRequest? queryString = null,
        Dictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
        where TRequestBody : class
    {
        HttpContent response = await BaseHttpCall(action, HttpMethod.Put, queryString, headers, body, cancellationToken);
        return await response.ReadAsStringAsync(cancellationToken);
    }
}
