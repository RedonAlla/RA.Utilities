using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Authorization.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for authorization-related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the <see cref="UserContext"/> service and its required dependencies, such as <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/>.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddUserContext() =>
            services.AddScoped<IUserContext, UserContext>();
    }
}
