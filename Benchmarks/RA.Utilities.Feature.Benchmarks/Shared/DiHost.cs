using System;
using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Feature.Benchmarks.Shared;

/// <summary>
/// Builds a service provider for a benchmark scenario and exposes its scope factory.
/// The returned factory keeps the provider alive for the lifetime of the benchmark run.
/// </summary>
internal static class DiHost
{
    public static IServiceScopeFactory BuildScopeFactory(Func<IServiceCollection, IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        ServiceProvider provider = configure(services).BuildServiceProvider();
        return provider.GetRequiredService<IServiceScopeFactory>();
    }
}
