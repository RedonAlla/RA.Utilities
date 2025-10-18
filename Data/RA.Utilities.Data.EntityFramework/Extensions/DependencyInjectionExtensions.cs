using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Data.Abstractions;

namespace RA.Utilities.Data.EntityFramework.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for data-related services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IReadRepositoryBase{TEntity}"/> and its implementation <see cref="ReadRepositoryBase{TEntity}"/>
    /// for the specified DbContext to the <see cref="IServiceCollection"/> as a scoped service.
    /// 
    /// The DI container will automatically resolve the correct 'T' at runtime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddReadRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IReadRepositoryBase<>), typeof(ReadRepositoryBase<>));
    }

    /// <summary>
    /// Adds the <see cref="IWriteRepositoryBase{TEntity}"/> and its implementation <see cref="WriteRepositoryBase{TEntity}"/>
    /// for the specified DContext to the <see cref="IServiceCollection"/> as a scoped service.
    /// 
    /// The DI container will automatically resolve the correct 'T' at runtime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddWriteRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IWriteRepositoryBase<>), typeof(WriteRepositoryBase<>));
    }

    /// <summary>
    /// Adds the <see cref="IRepositoryBase{TEntity}"/> and its implementation <see cref="RepositoryBase{TEntity}"/>
    /// for the specified DContext to the <see cref="IServiceCollection"/> as a scoped service.
    /// 
    /// The DI container will automatically resolve the correct 'T' at runtime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRepositoryBase(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
    }
}
