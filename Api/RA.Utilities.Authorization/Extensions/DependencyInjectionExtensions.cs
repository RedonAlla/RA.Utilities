using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Authorization.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for authorization-related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the <see cref="AppUser"/> service and its required dependencies, such as <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAppUser(this IServiceCollection services) =>
        services.AddTransient<AppUser>()
                .AddHttpContextAccessor();
}
