using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Dispatcher.Abstractions;

/// <summary>
/// Defines a contract for configuring request and notification dispatching.
/// </summary>
public interface IDispatcherBuilder
{
    /// <summary>
    /// Gets the collection of services used to configure dispatching.
    /// </summary>
    IServiceCollection Services { get; }
}
