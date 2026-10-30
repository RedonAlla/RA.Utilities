using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Options;

namespace RA.Utilities.Dispatcher.Generated;

/// <summary>
/// Queue of handler-registration callbacks produced by the compile-time registration
/// source generator (one entry per referencing assembly, added from each assembly's
/// module initializer). <see cref="ApplyAll"/> is invoked by <c>AddDispatcher</c> so that
/// handlers are registered without explicit configuration.
/// </summary>
public static class HandlerRegistrations
{
    private static readonly Lock Sync = new();
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
    /// Applies all queued registrations to the specified service collection, optionally filtering
    /// the auto-registered handlers and their related behaviors.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="options">
    /// The include/exclude options, or <see langword="null"/> to register everything.
    /// </param>
    internal static void ApplyAll(IServiceCollection services, DispatcherOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        Action<IServiceCollection>[] snapshot;
        lock (Sync)
        {
            snapshot = [.. Registrations];
        }

        int startIndex = services.Count;
        foreach (Action<IServiceCollection> registration in snapshot)
        {
            registration(services);
        }

        if (options is not null)
        {
            HandlerRegistrationFilter.Apply(services, startIndex, options);
        }
    }
}
