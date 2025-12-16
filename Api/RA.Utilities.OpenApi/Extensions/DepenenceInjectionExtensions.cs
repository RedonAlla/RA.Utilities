using Microsoft.AspNetCore.OpenApi;
using RA.Utilities.OpenApi.DocumentTransformers;
using RA.Utilities.OpenApi.SchemaTransformers;

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

    /// <summary>
    /// Adds schema transformers that incorporate FluentValidation rules into the OpenAPI document.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/>.</returns>
    public static OpenApiOptions AddFluentValidationRules(this OpenApiOptions options) =>
        options.AddSchemaTransformer<FluentValidationSchemaTransformer>();

    /// <summary>
    /// Adds a schema transformer that enriches enum types in the OpenAPI schema with XML documentation descriptions.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <param name="xmlPath">The path to the XML documentation file containing enum descriptions.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/>.</returns>
    public static OpenApiOptions AddEnumXmlDescriptionTransformer(this OpenApiOptions options, string xmlPath) =>
        options.AddSchemaTransformer(new EnumXmlSchemaTransformer(xmlPath));

    /// <summary>
    /// Adds the <see cref="DefaultResponsesOperationTransformer"/> to the OpenAPI options.
    /// This transformer adds default error responses to all operations.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddDefaultResponsesOperationTransformer(this OpenApiOptions options) =>
        options.AddOperationTransformer<DefaultResponsesOperationTransformer>();

    /// <summary>
    /// Adds the <see cref="DocumentInfoTransformer"/> to the OpenAPI options.
    /// This transformer populates the document's Info object from configuration settings.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddDocumentInfoTransformer(this OpenApiOptions options) =>
        options.AddDocumentTransformer<DocumentInfoTransformer>();

    /// <summary>
    /// Adds the <see cref="BearerSecuritySchemeTransformer"/> to the OpenAPI options.
    /// This transformer adds a Bearer security scheme if JWT authentication is detected.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddBearerSecuritySchemeTransformer(this OpenApiOptions options) =>
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

    /// <summary>
    /// Adds the <see cref="HeadersParameterTransformer"/> to the OpenAPI options.
    /// This transformer adds configured request and response headers to all operations.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddHeadersParameterTransformer(this OpenApiOptions options) =>
        options.AddDocumentTransformer<HeadersParameterTransformer>();

    /// <summary>
    /// Adds the <see cref="PolymorphismSchemaFilter"/> to the OpenAPI options.
    /// This filter supports polymorphic schemas by adding derived types and configuring a discriminator.
    /// </summary>
    /// <param name="options">The <see cref="OpenApiOptions"/> to configure.</param>
    /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
    public static OpenApiOptions AddPolymorphismSchemaFilter(this OpenApiOptions options) =>
        options.AddDocumentTransformer<PolymorphismSchemaFilter>();
}
