using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Data.Abstractions;

namespace RA.Utilities.Data.EntityFramework.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for data-related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the generic <see cref="IReadRepositoryBase{T}"/> and its implementation <see cref="ReadRepositoryBase{T}"/>
    /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
    /// `IReadRepositoryBase&lt;TEntity&gt;` for any entity `TEntity` that inherits from `BaseEntity`.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddReadRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IReadRepositoryBase<>), typeof(ReadRepositoryBase<>));
    }

    /// <summary>
    /// Adds the generic <see cref="IWriteRepositoryBase{T}"/> and its implementation <see cref="WriteRepositoryBase{T}"/>
    /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
    /// `IWriteRepositoryBase&lt;TEntity&gt;` for any entity `TEntity` that inherits from `BaseEntity`.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddWriteRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IWriteRepositoryBase<>), typeof(WriteRepositoryBase<>));
    }

    /// <summary>
    /// Adds the generic <see cref="IRepositoryBase{T}"/> and its implementation <see cref="RepositoryBase{T}"/>
    /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
    /// `IRepositoryBase&lt;TEntity&gt;` for any entity `TEntity` that inherits from `BaseEntity`.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
    }
}
