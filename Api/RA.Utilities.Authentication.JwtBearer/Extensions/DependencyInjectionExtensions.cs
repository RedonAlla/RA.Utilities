using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RA.Utilities.Authentication.JwtBearer.Configurations;

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
    ///   <item>Adds authorization services via <c>AddAuthorization()</c>.</item>
    ///   <item>Configures authentication with JWT Bearer as the default scheme.</item>
    ///   <item>
    ///     Registers <see cref="ConfigureJwtBearerOptions"/> which handles all
    ///     <see cref="JwtBearerOptions"/> binding and special conversions.
    ///     The optional <paramref name="configureOptions"/> callback is invoked
    ///     <em>last</em>, so it can override any config-driven values.
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/> instance.</param>
    /// <param name="configureOptions">An optional action to further customize <see cref="JwtBearerOptions"/> after all configuration binding has been applied.</param>
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
            }).AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(
            new ConfigureJwtBearerOptions(configuration, configureOptions));

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
