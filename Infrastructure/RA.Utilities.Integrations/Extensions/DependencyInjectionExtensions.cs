using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RA.Utilities.Integrations.Abstractions;
using RA.Utilities.Integrations.DelegatingHandlers;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Extensions methods for registering HTTP Clients and HttMessageHandlers.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers and configures a typed HttpClient for a specific integration.
    /// </summary>
    /// <remarks>
    /// This method performs several actions:
    /// 1. Binds the provided <paramref name="configurationSection"/> to the <typeparamref name="TSettings"/> class.
    /// 2. Validates the settings instance using its DataAnnotations. Validation occurs on application start.
    /// 3. Registers a typed <see cref="HttpClient"/> for the specified <typeparamref name="TClient"/>.
    /// 4. Configures the HttpClient's BaseAddress and Timeout from the settings.
    /// </remarks>
    /// <typeparam name="TInterface">The interface type of the client to register.</typeparam>
    /// <typeparam name="TClient">The typed client class to register.</typeparam>
    /// <typeparam name="TSettings">The settings class that implements <see cref="IIntegrationSettings"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configurationSection">The configuration section to bind the settings from.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the client.</returns>
    public static IHttpClientBuilder AddHttpClientIntegration<TInterface, TClient, TSettings>(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
        where TInterface : class
        where TClient : class, TInterface
        where TSettings : class, IIntegrationSettings
    {
        services.AddOptions<TSettings>()
            .Bind(configurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services.AddHttpClient<TInterface, TClient>((serviceProvider, httpClient) =>
            {
                TSettings settings = serviceProvider.GetRequiredService<IOptions<TSettings>>().Value;
                httpClient.BaseAddress = settings.BaseUrl;
                httpClient.Timeout = TimeSpan.FromSeconds(settings.Timeout);
            });
    }

    /// <summary>
    /// Registers a configuration instance of <typeparamref name="T"/> with DataAnnotations validations on startup,
    /// if configuration does not exists.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="key">If parameter key it is empty it will try to read key by class name.</param>
    public static void AddOptionWithValidations<T>(this IServiceCollection services, string? key = null) where T : class
    {
        services.AddOptions<T>()
                .BindConfiguration(key ?? typeof(T).Name) // 👈 Bind the key section
                .ValidateDataAnnotations()                // 👈 Enable validation
                .ValidateOnStart();                       // 👈 Validate on app start
    }

    /// <summary>
    /// Registers a scoped HTTP message handler of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the delegating handler.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddScopedHttpMessageHandler<T>(this IServiceCollection services) where T : DelegatingHandler =>
        services.AddScoped<T>();

    /// <summary>
    /// Registers a transient HTTP message handler of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the delegating handler.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddTransientHttpMessageHandler<T>(this IServiceCollection services) where T : DelegatingHandler =>
        services.AddTransient<T>();

    /// <summary>
    /// Add <see cref="RequestResponseLoggingHandler"/> for logging HTTP Request / Response for this client.
    /// </summary>
    /// <param name="httpClientBuilder"><see cref="IHttpClientBuilder"/></param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used  to configure the client.</returns>
    public static IHttpClientBuilder WithHttpLoggingHandler(this IHttpClientBuilder httpClientBuilder) =>
        httpClientBuilder.AddHttpMessageHandler<RequestResponseLoggingHandler>();

    /// <summary>
    /// Add <see cref="InternalHeadersForwardHandler"/> to add automatic
    /// access token and x-request-id for each HTTP call for this client.
    /// </summary>
    /// <param name="httpClientBuilder"><see cref="IHttpClientBuilder"/></param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used  to configure the client.</returns>
    public static IHttpClientBuilder WithInternalHeadersForwardingHandler(this IHttpClientBuilder httpClientBuilder) =>
        httpClientBuilder.AddHttpMessageHandler<InternalHeadersForwardHandler>();

    /// <summary>
    /// Adds a static API key to the DefaultRequestHeaders of the HTTP client.
    /// </summary>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="apiKey">The API key value.</param>
    /// <param name="headerName">The name of the header to add the API key to. Defaults to "X-Api-Key".</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder WithApiKey(this IHttpClientBuilder httpClientBuilder, string apiKey, string headerName = "X-Api-Key")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(headerName);

        httpClientBuilder.ConfigureHttpClient(client => client.DefaultRequestHeaders.Add(headerName, apiKey));

        return httpClientBuilder;
    }

    /// <summary>
    /// Adds a delegating handler that injects an API key to the "X-Api-Key" header from a configured settings object.
    /// The settings object must be registered in DI and implement <see cref="IApiKeySettings"/>.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings object that holds the ApiKey.</typeparam>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
    public static IHttpClientBuilder WithApiKeyFromSettingsHandler<TSettings>(this IHttpClientBuilder httpClientBuilder)
        where TSettings : class, IApiKeySettings
    {
        // The handler needs to be registered so DI can create it when the pipeline is built.
        // It's registered as transient because HttpMessageHandler instances are created and managed
        // by the IHttpClientFactory, which creates a new pipeline for each client instance.
        _ = httpClientBuilder.Services.AddTransient<ApiKeyAuthenticationHandler<TSettings>>();
        return httpClientBuilder.AddHttpMessageHandler<ApiKeyAuthenticationHandler<TSettings>>();
    }

    /// <summary>
    /// Configures the primary HttpMessageHandler to use a proxy based on settings.
    /// </summary>
    /// <typeparam name="TSettings">The type of the root settings object.</typeparam>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/>.</param>
    /// <param name="proxySettingsSelector">A function to select the proxy settings from the root settings object.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the client.</returns>
    public static IHttpClientBuilder WithProxyFromSettings<TSettings>(
        this IHttpClientBuilder httpClientBuilder,
        Func<TSettings, IProxySettings> proxySettingsSelector)
        where TSettings : class
    {
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(serviceProvider =>
        {
            TSettings settings = serviceProvider.GetRequiredService<IOptions<TSettings>>().Value;
            IProxySettings proxySettings = proxySettingsSelector(settings);

            // The logic for creating the handler is now encapsulated in the ProxyMessageHandler.
            return ProxyMessageHandler.Create(proxySettings);
        });

        return httpClientBuilder;
    }
}
