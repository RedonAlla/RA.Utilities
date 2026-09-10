using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Feature.Generated;

/// <summary>
/// Queue of handler-registration callbacks produced by the compile-time registration
/// source generator (one entry per referencing assembly, added from each assembly's
/// module initializer). <see cref="ApplyAll"/> is invoked by <c>AddMediator</c> so that
/// handlers are registered without explicit configuration.
/// </summary>
public static class HandlerRegistrations
{
    private static readonly object Sync = new();
    private static readonly List<Action<IServiceCollection>> Registrations = [];

    /// <summary>
    /// Queues a handler-registration callback. Called by generated module initializers.
    /// </summary>
    /// <param name="registration">The registration callback.</param>
    public static void Add(Action<IServiceCollection> registration)
    {
        ArgumentNullException.ThrowIfNull(registration);
        lock (Sync)
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
        lock (Sync)
        {
            snapshot = [.. Registrations];
        }

        foreach (Action<IServiceCollection> registration in snapshot)
        {
            registration(services);
        }
    }
}
