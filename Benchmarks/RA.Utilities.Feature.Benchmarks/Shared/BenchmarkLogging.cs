using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RA.Utilities.Feature.Benchmarks.Shared;

/// <summary>
/// Logging registration shared by the benchmark scenarios. The package behaviors log at
/// Information/Debug/Warning levels; the <see cref="NullLoggerFactory"/> disables every level, so
/// the <c>Log*</c> extension calls short-circuit at the <c>IsEnabled</c> check before any message
/// formatting or destructuring happens — the benchmark measures the behavior's own overhead, not
/// a logging sink.
/// </summary>
internal static class BenchmarkLogging
{
    /// <summary>
    /// Registers the null logger factory and an open-generic <c>ILogger&lt;T&gt;</c>
    /// implementation so that every closed logger requested by the behaviors (and by the
    /// generated mediator) resolves to a no-op logger backed by <see cref="NullLoggerFactory"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection so that additional calls can be chained.</returns>
    public static IServiceCollection AddNullLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        return services;
    }
}
