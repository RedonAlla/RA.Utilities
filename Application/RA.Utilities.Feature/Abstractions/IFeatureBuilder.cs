using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// Defines a contract for building features within the application.
/// </summary>
public interface IFeatureBuilder
{
    /// <summary>
    /// Gets the collection of services registered in the application.
    /// </summary>
    IServiceCollection Services { get; }
}
