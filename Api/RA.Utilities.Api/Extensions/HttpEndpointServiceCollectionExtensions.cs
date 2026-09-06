namespace RA.Utilities.Api.Extensions;

/// <summary>
/// Provides the compile-time endpoint registration pipeline. The
/// <c>RA.Utilities.Api.Generators</c> source generator (shipped inside this package as an analyzer)
/// extends this partial class in the consuming assembly with a <c>MapEndpoints</c> extension method
/// that discovers every <see cref="RA.Utilities.Api.Abstractions.IEndpointGroup"/> and
/// <see cref="RA.Utilities.Api.Abstractions.IEndpoint"/> implementation at compile time.
/// </summary>
/// <remarks>
/// The <c>MapEndpoints</c> method itself is emitted entirely by the generator: a public partial
/// method cannot span assemblies (the defining declaration would require an implementation part in
/// this assembly, and having two implementations would make extension method calls ambiguous), so
/// the generated code owns the whole method.
/// </remarks>
public static partial class HttpEndpointServiceCollectionExtensions
{
}
