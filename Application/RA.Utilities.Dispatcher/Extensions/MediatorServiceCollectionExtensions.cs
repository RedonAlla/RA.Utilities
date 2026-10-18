using System;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Generated;

namespace RA.Utilities.Dispatcher.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to add Mediator services.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds Mediator services to the specified <see cref="IServiceCollection"/> and applies
        /// the compile-time handler, behavior, and mediator registrations produced by the
        /// <c>RA.Utilities.Dispatcher.Generators</c> source generator. The generated
        /// <c>Mediator</c> becomes the <c>IDispatcher</c> implementation.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public IServiceCollection AddMediator()
        {
            ArgumentNullException.ThrowIfNull(services);

            HandlerRegistrations.ApplyAll(services);
            MediatorRegistrations.ApplyAll(services);

            return services;
        }
    }
}
