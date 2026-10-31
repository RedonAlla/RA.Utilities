using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RA.Utilities.Dispatcher.Abstractions;
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
        /// source generator, filtered by the supplied options, and registering the pipeline behaviors
        /// listed in <see cref="DispatcherOptions.Behaviors"/>.
        /// </summary>
        /// <param name="configure">
        /// Configures which auto-registered handlers and related behaviors are kept, and which
        /// additional pipeline behaviors are registered.
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
        var configuredBehaviors = new List<(Type ServiceType, Type ImplementationType)>();
        if (options is not null)
        {
            AddConfiguredBehaviors(services, options, configuredBehaviors);
        }

        int generatedStartIndex = services.Count;
        HandlerRegistrations.ApplyAll(services, options);
        DispatcherRegistrations.ApplyAll(services);

        RemoveGeneratedBehaviorDuplicates(services, generatedStartIndex, configuredBehaviors);

        return services;
    }

    /// <summary>
    /// Registers the behaviors listed in <see cref="DispatcherOptions.Behaviors"/> before the generated
    /// registrations, so that they form the outer part of every chain they participate in.
    /// </summary>
    private static void AddConfiguredBehaviors(
        IServiceCollection services,
        DispatcherOptions options,
        List<(Type ServiceType, Type ImplementationType)> registered)
    {
        foreach (Type behaviorType in options.Behaviors)
        {
            ArgumentNullException.ThrowIfNull(behaviorType);

            if (!behaviorType.IsClass || behaviorType.IsAbstract)
            {
                throw new ArgumentException(
                    $"Pipeline behavior '{behaviorType}' must be a non-abstract class.", nameof(options));
            }

            Type[] serviceTypes = GetBehaviorServiceTypes(behaviorType);
            if (serviceTypes.Length == 0)
            {
                throw new ArgumentException(
                    $"Type '{behaviorType}' does not implement IPipelineBehavior<TRequest>, IPipelineBehavior<TRequest, TResponse>, or INotificationBehavior<TNotification>.",
                    nameof(options));
            }

            foreach (Type serviceType in serviceTypes)
            {
                services.TryAddEnumerable(ServiceDescriptor.Describe(serviceType, behaviorType, options.BehaviorLifetime));
                registered.Add((serviceType, behaviorType));
            }
        }
    }

    /// <summary>
    /// Returns the behavior interfaces implemented by <paramref name="behaviorType"/>, using the open
    /// generic definition for an open generic implementation.
    /// </summary>
    private static Type[] GetBehaviorServiceTypes(Type behaviorType)
    {
        List<Type>? serviceTypes = null;
        Type[]? implementationArguments = behaviorType.IsGenericTypeDefinition
            ? behaviorType.GetGenericArguments()
            : null;

        foreach (Type implementedInterface in behaviorType.GetInterfaces())
        {
            if (!implementedInterface.IsGenericType)
            {
                continue;
            }

            Type definition = implementedInterface.GetGenericTypeDefinition();
            if (definition != typeof(IPipelineBehavior<>)
                && definition != typeof(IPipelineBehavior<,>)
                && definition != typeof(INotificationBehavior<>))
            {
                continue;
            }

            if (implementationArguments is null)
            {
                (serviceTypes ??= []).Add(implementedInterface);
                continue;
            }

            // An open generic implementation can only be registered under the open behavior
            // interface when it passes its own type parameters through, in order. Partially closed
            // mappings (for example IPipelineBehavior<ConcreteRequest, T>) are not supported by DI.
            if (!HasMatchingTypeArguments(implementedInterface, implementationArguments))
            {
                throw new ArgumentException(
                    $"Open generic behavior '{behaviorType}' must pass its own type parameters, in order, to '{definition}'.",
                    nameof(behaviorType));
            }

            (serviceTypes ??= []).Add(definition);
        }

        return serviceTypes is null ? [] : [.. serviceTypes];
    }

    private static bool HasMatchingTypeArguments(Type implementedInterface, Type[] implementationArguments)
    {
        Type[] interfaceArguments = implementedInterface.GetGenericArguments();
        if (interfaceArguments.Length != implementationArguments.Length)
        {
            return false;
        }

        for (int i = 0; i < interfaceArguments.Length; i++)
        {
            if (interfaceArguments[i] != implementationArguments[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Removes the generated descriptors for behaviors that were also listed explicitly in
    /// <see cref="DispatcherOptions.Behaviors"/>. The configured registration is kept because it was
    /// added first and therefore sits at the configured (outer) position in the chain.
    /// </summary>
    private static void RemoveGeneratedBehaviorDuplicates(
        IServiceCollection services,
        int startIndex,
        List<(Type ServiceType, Type ImplementationType)> configuredBehaviors)
    {
        if (configuredBehaviors.Count == 0)
        {
            return;
        }

        for (int i = services.Count - 1; i >= startIndex; i--)
        {
            ServiceDescriptor descriptor = services[i];
            if (descriptor.ImplementationType is Type implementationType
                && configuredBehaviors.Contains((descriptor.ServiceType, implementationType)))
            {
                services.RemoveAt(i);
            }
        }
    }
}
