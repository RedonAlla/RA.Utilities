using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Behaviors;

namespace RA.Utilities.Dispatcher.Builders;

/// <summary>
/// A builder for configuring dispatch for a request without a response.
/// </summary>
public class DispatcherBuilder<TRequest> : IDispatcherBuilder where TRequest : IRequest
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherBuilder{TRequest}"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public DispatcherBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Adds a decoration (pipeline behavior) to the dispatch pipeline.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the pipeline behavior.</typeparam>
    /// <returns>The current dispatcher builder instance.</returns>
    public DispatcherBuilder<TRequest> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest>
    {
        Services.AddTransient<IPipelineBehavior<TRequest>, TBehavior>();
        return this;
    }

    /// <summary>
    /// Adds a validator to the dispatch pipeline.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <returns>The current dispatcher builder instance.</returns>
    public DispatcherBuilder<TRequest> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>
    {
        Services.AddTransient<IValidator<TRequest>, TValidator>();
        Services.AddTransient<IPipelineBehavior<TRequest>, ValidationBehavior<TRequest>>();
        return this;
    }
}

/// <summary>
/// A builder for configuring dispatch for a request with a response.
/// </summary>
public class DispatcherBuilder<TRequest, TResponse> : IDispatcherBuilder where TRequest : IRequest<TResponse>
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DispatcherBuilder{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public DispatcherBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Adds a decoration (pipeline behavior) to the dispatch pipeline.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the pipeline behavior.</typeparam>
    /// <returns>The current dispatcher builder instance.</returns>
    public DispatcherBuilder<TRequest, TResponse> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest, TResponse>
    {
        Services.AddTransient<IPipelineBehavior<TRequest, TResponse>, TBehavior>();
        return this;
    }

    /// <summary>
    /// Adds a validator to the dispatch pipeline.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <returns>The current dispatcher builder instance.</returns>
    public DispatcherBuilder<TRequest, TResponse> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>
    {
        Services.AddTransient<IValidator<TRequest>, TValidator>();
        Services.AddTransient<IPipelineBehavior<TRequest, TResponse>, ValidationBehavior<TRequest, TResponse>>();
        return this;
    }
}
