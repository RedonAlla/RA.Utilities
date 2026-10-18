using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RA.Utilities.Dispatcher.Benchmarks.Shared;

/// <summary>
/// Logging registration shared by the benchmark scenarios. The generated mediators and the
/// package behaviors resolve <c>ILogger&lt;T&gt;</c>; the <see cref="NullLoggerFactory"/> disables
/// every level, so log calls short-circuit at the <c>IsEnabled</c> check before any message
/// formatting or destructuring happens — the benchmark measures dispatch overhead, not a logging
/// sink.
/// </summary>
internal static class BenchmarkLogging
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the null logger factory and an open-generic <c>ILogger&lt;T&gt;</c>
        /// implementation so that every closed logger requested by the generated mediators
        /// resolves to a no-op logger backed by <see cref="NullLoggerFactory.Instance"/>.
        /// </summary>
        /// <returns>The service collection so that additional calls can be chained.</returns>
        public IServiceCollection AddNullLogging()
        {
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            return services;
        }
    }
}
