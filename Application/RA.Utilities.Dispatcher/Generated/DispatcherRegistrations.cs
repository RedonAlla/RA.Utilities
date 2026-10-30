using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Dispatcher.Generated;

/// <summary>
/// Queue of dispatcher-registration callbacks produced by the dispatcher source generator
/// (one entry per referencing assembly, added from each assembly's module initializer).
/// <see cref="ApplyAll"/> is invoked by <c>AddDispatcher</c> so that the generated
/// <c>Dispatcher</c> becomes the <see cref="Abstractions.IDispatcher"/> implementation
/// without explicit configuration.
/// </summary>
public static class DispatcherRegistrations
{
    private static readonly Lock _sync = new();
    private static readonly List<Action<IServiceCollection>> Registrations = [];

    /// <summary>
    /// Queues a dispatcher-registration callback. Called by generated module initializers.
    /// </summary>
    /// <param name="registration">The registration callback.</param>
    public static void Add(Action<IServiceCollection> registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        lock (_sync)
        {
            Registrations.Add(registration);
        }
    }

    /// <summary>
    /// Applies all queued registrations to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    internal static void ApplyAll(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        Action<IServiceCollection>[] snapshot;
        lock (_sync)
        {
            snapshot = [.. Registrations];
        }

        foreach (Action<IServiceCollection> registration in snapshot)
        {
            registration(services);
        }
    }
}
