using Microsoft.AspNetCore.Builder;

namespace RA.Utilities.Authentication.JwtBearer.Extensions;

/// <summary>
/// Provides extension methods for simplifying the setup of JWT Bearer authentication
/// and authorization in an ASP.NET Core application.
/// </summary>
public static class ApplicationBuilderExtensions
{
    extension(IApplicationBuilder app)
    {
        /// <summary>
        /// Adds the authentication and authorization middleware to the application's request pipeline.
        /// </summary>
        /// <remarks>
        /// This is a convenience method that chains <c>app.UseAuthentication()</c> and <c>app.UseAuthorization()</c>.
        /// It must be called in the correct order in your `Program.cs`: after routing and before endpoint mapping.
        /// </remarks>
        /// <returns>The <see cref="IApplicationBuilder"/> for chaining.</returns>
        public IApplicationBuilder UseAuth()
        {
            return app
                .UseAuthentication()
                .UseAuthorization();
        }
    }
}
