using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Behaviors;

namespace RA.Utilities.Feature.Builders;

/// <summary>
/// A builder for configuring a feature (request without a response).
/// </summary>
public class FeatureBuilder<TRequest> : IFeatureBuilder where TRequest : IRequest
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureBuilder{TRequest}"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public FeatureBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Adds a decoration (pipeline behavior) to the feature.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the pipeline behavior.</typeparam>
    /// <returns>The current feature builder instance.</returns>
    public FeatureBuilder<TRequest> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest>
    {
        Services.AddTransient<IPipelineBehavior<TRequest>, TBehavior>();
        return this;
    }

    /// <summary>
    /// Adds a validator to the feature.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <returns>The current feature builder instance.</returns>
    public FeatureBuilder<TRequest> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>
    {
        Services.AddTransient<IValidator<TRequest>, TValidator>();
        Services.AddTransient<IPipelineBehavior<TRequest>, ValidationBehavior<TRequest>>();
        return this;
    }
}

/// <summary>
/// A builder for configuring a feature (request with a response).
/// </summary>
public class FeatureBuilder<TRequest, TResponse> : IFeatureBuilder where TRequest : IRequest<TResponse>
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureBuilder{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public FeatureBuilder(IServiceCollection services) => Services = services;

    /// <summary>
    /// Adds a decoration (pipeline behavior) to the feature.
    /// </summary>
    /// <typeparam name="TBehavior">The type of the pipeline behavior.</typeparam>
    /// <returns>The current feature builder instance.</returns>
    public FeatureBuilder<TRequest, TResponse> AddDecoration<TBehavior>() where TBehavior : class, IPipelineBehavior<TRequest, TResponse>
    {
        Services.AddTransient<IPipelineBehavior<TRequest, TResponse>, TBehavior>();
        return this;
    }

    /// <summary>
    /// Adds a validator to the feature.
    /// </summary>
    /// <typeparam name="TValidator">The type of the validator.</typeparam>
    /// <returns>The current feature builder instance.</returns>
    public FeatureBuilder<TRequest, TResponse> AddValidator<TValidator>() where TValidator : class, IValidator<TRequest>
    {
        Services.AddTransient<IValidator<TRequest>, TValidator>();
        Services.AddTransient<IPipelineBehavior<TRequest, TResponse>, ValidationBehavior<TRequest, TResponse>>();
        return this;
    }
}
