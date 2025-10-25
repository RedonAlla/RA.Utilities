using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RA.Utilities.Authentication.JwtBearer.Configurations;
using RA.Utilities.Authentication.JwtBearer.Constants;

namespace RA.Utilities.Authentication.JwtBearer.Extensions;

/// <summary>
/// Provides extension methods for simplifying the setup of JWT Bearer authentication
/// and authorization in an ASP.NET Core application.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds and configures JWT Bearer authentication services using settings from the application's configuration.
    /// </summary>
    /// <remarks>
    /// This is the primary setup method for the library. It performs the following actions:
    /// <list type="bullet">
    ///   <item>Binds <see cref="JwtBearerOptions"/> to the <c>Authentication:Schemes:Bearer</c> configuration section.</item>
    ///   <item>Registers a custom configuration class to handle the <c>IssuerSigningKeyString</c>.</item>
    ///   <item>Adds the necessary authorization services via <c>AddAuthorization()</c>.</item>
    /// </list>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/> instance.</param>
    /// <param name="configureOptions">An optional action to further customize <see cref="JwtBearerOptions"/> after initial configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                configuration.GetSection(KeyConstants.JwtBearerOptionsKey).Bind(options);
                configureOptions?.Invoke(options);
            });

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        return services;
    }

    /// <summary>
    /// Adds the authentication and authorization middleware to the application's request pipeline.
    /// </summary>
    /// <remarks>
    /// This is a convenience method that chains <c>app.UseAuthentication()</c> and <c>app.UseAuthorization()</c>.
    /// It must be called in the correct order in your `Program.cs`: after routing and before endpoint mapping.
    /// </remarks>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        return app
            .UseAuthentication()
            .UseAuthorization();
    }
}
