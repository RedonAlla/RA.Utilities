using Microsoft.AspNetCore.OpenApi;
using RA.Utilities.OpenApi.DocumentTransformers;

namespace RA.Utilities.OpenApi.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI services.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the default set of document transformers.
    /// </summary>
    /// <remarks>
    /// This method performs the following actions:
    /// <list type="number">
    ///   <item>
    ///     <description>Adds a default set of document transformers using <see cref="AddDefaultsDocumentTransformer"/>:
    ///       <list type="bullet">
    ///         <item><description><see cref="DocumentInfoTransformer"/>: Populates document info from configuration.</description></item>
    ///         <item><description><see cref="BearerSecuritySchemeTransformer"/>: Adds a Bearer token security scheme if JWT authentication is present.</description></item>
    ///         <item><description><see cref="HeadersParameterTransformer"/>: Adds configured request and response headers to all operations.</description></item>
    ///       </list>
    ///     </description>
    ///   </item>
    /// </list>
    /// See the package README for details on configuring <c>appsettings.json</c>.
    /// </remarks>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddDefaultsDocumentTransformer(this OpenApiOptions options)
    {
        options?.AddDocumentTransformer<DocumentInfoTransformer>();
        options?.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        options?.AddDocumentTransformer<HeadersParameterTransformer>();
        return options!;
    }
}
