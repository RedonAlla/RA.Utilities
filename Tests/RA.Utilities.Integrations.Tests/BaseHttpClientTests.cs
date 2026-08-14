using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using RA.Utilities.Integrations;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.Tests.Models;

namespace RA.Utilities.Integrations.Tests;

/// <summary>
/// Contains end-to-end tests for the <see cref="BaseHttpClient"/> class, using a stub
/// message handler and strongly-typed query/header parameter classes generated at build time.
/// </summary>
public class BaseHttpClientTests
{
    /// <summary>
    /// A stub <see cref="HttpMessageHandler"/> that captures the outgoing request
    /// and returns a canned response.
    /// </summary>
    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// Gets the HTTP method of the last request.
        /// </summary>
        public HttpMethod? Method { get; private set; }

        /// <summary>
        /// Gets the request URI of the last request.
        /// </summary>
        public Uri? RequestUri { get; private set; }

        /// <summary>
        /// Gets the headers of the last request as a snapshot.
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the body of the last request, or <see langword="null"/> if there was none.
        /// </summary>
        public string? Body { get; private set; }

        /// <summary>
        /// Gets or sets the content type of the last request.
        /// </summary>
        public string? ContentType { get; private set; }

        /// <summary>
        /// Gets or sets the response body to return.
        /// </summary>
        public string ResponseBody { get; set; } = "{}";

        /// <summary>
        /// Gets or sets the status code to return.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Method = request.Method;
            RequestUri = request.RequestUri;
            ContentType = request.Content?.Headers.ContentType?.ToString();
            Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

            Headers.Clear();

            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                Headers[header.Key] = string.Join(",", header.Value);
            }

            return new HttpResponseMessage(StatusCode) { Content = new StringContent(ResponseBody) };
        }
    }

    /// <summary>
    /// A minimal <see cref="IIntegrationSettings"/> implementation for tests.
    /// </summary>
    private sealed class TestIntegrationSettings : IIntegrationSettings
    {
        public Uri BaseUrl { get; set; } = new("https://api.example.com/v1/");

        public bool UseProxy { get; set; }

        public double Timeout { get; set; } = 10;
    }

    /// <summary>
    /// A disposable test context owning the stub handler, the <see cref="HttpClient"/>
    /// and the <see cref="BaseHttpClient"/> under test.
    /// </summary>
    private sealed class TestClientContext : IDisposable
    {

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClientContext"/> class.
        /// </summary>
        /// <param name="handler">The stub handler.</param>
        public TestClientContext(StubHttpMessageHandler handler)
        {
            Handler = handler;
            _httpClient = new HttpClient(handler);
            Client = new BaseHttpClient(
                _httpClient,
                Microsoft.Extensions.Options.Options.Create<IIntegrationSettings>(new TestIntegrationSettings()));
        }

        /// <summary>
        /// Gets the stub handler.
        /// </summary>
        public StubHttpMessageHandler Handler { get; }

        /// <summary>
        /// Gets the client under test.
        /// </summary>
        public BaseHttpClient Client { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            _httpClient.Dispose();
            Handler.Dispose();
        }
    }

    /// <summary>
    /// Verifies that GET sends the query string and headers and deserializes the response.
    /// </summary>
    [Fact]
    public async Task GetAsync_ShouldSendGetRequest_WithQueryStringAndHeaders()
    {
        // Arrange
        using var context = new TestClientContext(
            new StubHttpMessageHandler { ResponseBody = """{"id": 5, "name": "widget"}""" });

        // Act
        Product? product = await context.Client.GetAsync<Product>(
            "products",
            new ProductQuery { CategoryId = 3, Search = "a b" },
            new ProductHeaders { XTraceId = "trace-1" });

        // Assert
        context.Handler.Method.Should().Be(HttpMethod.Get);
        GetRequestUri(context).Should().Be("https://api.example.com/v1/products?CategoryId=3&Search=a%20b");
        context.Handler.Headers.Should().Contain(KeyValuePair.Create("XTraceId", "trace-1"));
        context.Handler.Body.Should().BeNull();

        Assert.NotNull(product);
        product.Name.Should().Be("widget");
    }

    /// <summary>
    /// Verifies that POST sends a JSON body and deserializes the response.
    /// </summary>
    [Fact]
    public async Task PostAsync_ShouldSendJsonBody_WithContentType()
    {
        // Arrange
        using var context = new TestClientContext(
            new StubHttpMessageHandler { ResponseBody = """{"id": 9, "name": "created"}""" });
        var body = new Product { Id = 0, Name = "new widget" };

        // Act
        Product? product = await context.Client.PostAsync<Product, Product>("products", body);

        // Assert
        context.Handler.Method.Should().Be(HttpMethod.Post);
        GetRequestUri(context).Should().Be("https://api.example.com/v1/products");
        context.Handler.ContentType.Should().Be("application/json; charset=utf-8");
        context.Handler.Body.Should().Contain("\"id\": 0");
        context.Handler.Body.Should().Contain("\"name\": \"new widget\"");

        Assert.NotNull(product);
        product.Id.Should().Be(9);
    }

    /// <summary>
    /// Verifies that PUT sends a JSON body with query string and headers.
    /// </summary>
    [Fact]
    public async Task PutAsync_ShouldSendPutRequest_WithJsonBody_QueryStringAndHeaders()
    {
        // Arrange
        using var context = new TestClientContext(new StubHttpMessageHandler());

        // Act
        await context.Client.PutAsync<Product>(
            "products/7",
            new Product { Id = 7, Name = "updated" },
            new ProductQuery { CategoryId = 1 },
            new ProductHeaders { XTraceId = "trace-2" });

        // Assert
        context.Handler.Method.Should().Be(HttpMethod.Put);
        GetRequestUri(context).Should().Be("https://api.example.com/v1/products/7?CategoryId=1");
        context.Handler.Headers.Should().Contain(KeyValuePair.Create("XTraceId", "trace-2"));
        context.Handler.Body.Should().Contain("\"name\": \"updated\"");
    }

    /// <summary>
    /// Verifies that DELETE sends the query string and headers but no body.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldSendDeleteRequest_WithQueryStringAndHeaders_AndNoBody()
    {
        // Arrange
        using var context = new TestClientContext(new StubHttpMessageHandler());

        // Act
        await context.Client.DeleteAsync(
            "products",
            new ProductQuery { CategoryId = 2 },
            new ProductHeaders { XTraceId = "trace-3" });

        // Assert
        context.Handler.Method.Should().Be(HttpMethod.Delete);
        GetRequestUri(context).Should().Be("https://api.example.com/v1/products?CategoryId=2");
        context.Handler.Headers.Should().Contain(KeyValuePair.Create("XTraceId", "trace-3"));
        context.Handler.Body.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a request without query and header parameters sends a plain request
    /// without throwing.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithNullQueryAndHeaders_ShouldNotThrow()
    {
        // Arrange
        using var context = new TestClientContext(new StubHttpMessageHandler());

        // Act
        string response = await context.Client.GetAsync("products");

        // Assert
        GetRequestUri(context).Should().Be("https://api.example.com/v1/products");
        response.Should().Be("{}");
    }

    /// <summary>
    /// Verifies that a non-success status code throws an <see cref="HttpRequestException"/>.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithNonSuccessStatusCode_ShouldThrowHttpRequestException()
    {
        // Arrange
        using var context = new TestClientContext(
            new StubHttpMessageHandler { StatusCode = HttpStatusCode.NotFound });

        // Act
        Func<Task> act = () => context.Client.GetAsync("products");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    /// <summary>
    /// Verifies that an empty response body deserializes to <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithEmptyResponseBody_ShouldReturnNull()
    {
        // Arrange
        using var context = new TestClientContext(
            new StubHttpMessageHandler { ResponseBody = string.Empty });

        // Act
        Product? product = await context.Client.GetAsync<Product>("products/1");

        // Assert
        product.Should().BeNull();
    }

    /// <summary>
    /// Gets the request URI captured by the stub handler as it was constructed.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <returns>The original request URI string.</returns>
    private static string GetRequestUri(TestClientContext context) =>
        context.Handler.RequestUri?.OriginalString ?? string.Empty;
}
