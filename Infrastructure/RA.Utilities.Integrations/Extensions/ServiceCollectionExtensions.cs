using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RA.Utilities.Integrations.Abstractions;

namespace RA.Utilities.Integrations.Extensions;

/// <summary>
/// Extensions methods for registering HTTP Clients and HttMessageHandlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
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
        /// <param name="configurationSection">The configuration section to bind the settings from.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the client.</returns>
        public IHttpClientBuilder AddHttpClientIntegration<TInterface, TClient, TSettings>(
            IConfigurationSection configurationSection
        )
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
        /// Registers a scoped HTTP message handler of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the delegating handler.</typeparam>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddScopedHttpMessageHandler<T>() where T : DelegatingHandler =>
            services.AddScoped<T>();

        /// <summary>
        /// Registers a transient HTTP message handler of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the delegating handler.</typeparam>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddTransientHttpMessageHandler<T>() where T : DelegatingHandler =>
            services.AddTransient<T>();
    }
}
