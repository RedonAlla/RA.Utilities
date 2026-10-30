using System;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Generated;
using RA.Utilities.Dispatcher.Options;

namespace RA.Utilities.Dispatcher.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add Dispatcher services.
/// </summary>
public static class DispatcherServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Dispatcher services to the specified <see cref="IServiceCollection"/> and applies
        /// the compile-time handler, behavior, and dispatcher registrations produced by the
        /// <c>RA.Utilities.Dispatcher.Generators</c> source generator. The generated
        /// <c>Dispatcher</c> becomes the <c>IDispatcher</c> implementation.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddDispatcher()
        {
            ArgumentNullException.ThrowIfNull(services);

            return AddDispatcherCore(services, options: null);
        }

        /// <summary>
        /// Adds Dispatcher services to the specified <see cref="IServiceCollection"/>, applying the
        /// compile-time registrations produced by the <c>RA.Utilities.Dispatcher.Generators</c>
        /// source generator, filtered by the supplied options.
        /// </summary>
        /// <param name="configure">
        /// Configures which auto-registered handlers and related behaviors are kept.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddDispatcher(Action<DispatcherOptions> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            var options = new DispatcherOptions();
            configure(options);

            return AddDispatcherCore(services, options);
        }
    }

    private static IServiceCollection AddDispatcherCore(IServiceCollection services, DispatcherOptions? options)
    {
        HandlerRegistrations.ApplyAll(services, options);
        DispatcherRegistrations.ApplyAll(services);

        return services;
    }
}
