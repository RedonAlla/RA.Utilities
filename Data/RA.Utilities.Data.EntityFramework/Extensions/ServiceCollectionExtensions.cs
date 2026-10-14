using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Data.Abstractions;

namespace RA.Utilities.Data.EntityFramework.Extensions;

/// <summary>
/// Provides extension methods for setting up dependency injection for data-related services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the generic <see cref="IReadRepositoryBase{T, TKey}"/> and its implementation <see cref="ReadRepositoryBase{T, TKey}"/>
        /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
        /// `IReadRepositoryBase&lt;T, TKey&gt;` for any entity `T` that inherits from `CoreEntity&lt;TKey&gt;`.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddReadRepositoryBase() =>
            services.AddScoped(typeof(IReadRepositoryBase<,>), typeof(ReadRepositoryBase<,>));

        /// <summary>
        /// Adds the generic <see cref="IWriteRepositoryBase{T, TKey}"/> and its implementation <see cref="WriteRepositoryBase{T, TKey}"/>
        /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
        /// `IWriteRepositoryBase&lt;T, TKey&gt;` for any entity `T` that inherits from `CoreEntity&lt;TKey&gt;`.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddWriteRepositoryBase() =>
            services.AddScoped(typeof(IWriteRepositoryBase<,>), typeof(WriteRepositoryBase<,>));

        /// <summary>
        /// Adds the generic <see cref="IRepositoryBase{T, TKey}"/> and its implementation <see cref="RepositoryBase{T, TKey}"/>
        /// to the <see cref="IServiceCollection"/> as a scoped service. This allows for the injection of
        /// `IRepositoryBase&lt;T, TKey&gt;` for any entity `T` that inherits from `CoreEntity&lt;TKey&gt;`.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddRepositoryBase() =>
            services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
    }
}
