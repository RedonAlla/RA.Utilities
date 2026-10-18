using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Builders;

namespace RA.Utilities.Dispatcher.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to configure request and notification dispatching.
/// </summary>
public static class DispatcherBuilderExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds request-response dispatch to the service collection.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <typeparam name="THandler">The type of the handler for the request.</typeparam>
        /// <returns>A <see cref="DispatcherBuilder{TRequest, TResponse}"/> to configure the dispatch pipeline.</returns>
        public DispatcherBuilder<TRequest, TResponse> AddDispatcher<TRequest, TResponse, THandler>()
            where TRequest : IRequest<TResponse>
            where THandler : class, IRequestHandler<TRequest, TResponse>
        {
            services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
            return new DispatcherBuilder<TRequest, TResponse>(services);
        }

        /// <summary>
        /// Adds request-only dispatch to the service collection.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="THandler">The type of the handler for the request.</typeparam>
        /// <returns>A <see cref="DispatcherBuilder{TRequest}"/> to configure the dispatch pipeline.</returns>
        public DispatcherBuilder<TRequest> AddDispatcher<TRequest, THandler>()
            where TRequest : IRequest
            where THandler : class, IRequestHandler<TRequest>
        {
            services.AddScoped<IRequestHandler<TRequest>, THandler>();
            return new DispatcherBuilder<TRequest>(services);
        }

        /// <summary>
        /// Adds notification dispatching to the service collection.
        /// </summary>
        /// <typeparam name="TNotification">The type of the notification.</typeparam>
        /// <returns>A <see cref="NotificationDispatcherBuilder{TNotification}"/> to configure the notification.</returns>
        public NotificationDispatcherBuilder<TNotification> AddNotification<TNotification>()
            where TNotification : INotification
        {
            return new NotificationDispatcherBuilder<TNotification>(services);
        }
    }
}
