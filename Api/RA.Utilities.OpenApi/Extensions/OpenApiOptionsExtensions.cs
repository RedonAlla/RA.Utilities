using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.OpenApi;
using RA.Utilities.OpenApi.DocumentTransformers;
using RA.Utilities.OpenApi.OperationTransformers;
using RA.Utilities.OpenApi.SchemaTransformers;

namespace RA.Utilities.OpenApi.Extensions;

/// <summary>
/// Provides extension methods for configuring OpenAPI services.
/// </summary>
public static class OpenApiOptionsExtensions
{
    extension(OpenApiOptions options)
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
        ///         <item><description><see cref="BearerSecurityDocumentTransformer"/>: Adds a Bearer token security scheme if JWT authentication is present.</description></item>
        ///         <item><description><see cref="HeadersParameterTransformer"/>: Adds configured request and response headers to all operations.</description></item>
        ///         <item><description><see cref="HealthChecksDocumentTransformer"/>: Documents the health check endpoints.</description></item>
        ///       </list>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>Adds the <see cref="ValueObjectSchemaTransformer"/>, which documents the validation
        ///     constraints of the value objects from <c>RA.Utilities.Core.ValueObjects</c>.</description>
        ///   </item>
        /// </list>
        /// See the package README for details on configuring <c>appsettings.json</c>.
        /// </remarks>
        /// <returns>The <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddDefaultsDocumentTransformer()
        {
            ArgumentNullException.ThrowIfNull(options);
            options.AddDocumentTransformer<DocumentInfoTransformer>();
            options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();
            options.AddDocumentTransformer<HeadersParameterTransformer>();
            options.AddDocumentTransformer<HealthChecksDocumentTransformer>();
            options.AddSchemaTransformer<ValueObjectSchemaTransformer>();
            return options;
        }

        /// <summary>
        /// Adds a schema transformer that documents the validation constraints of the value objects
        /// from <c>RA.Utilities.Core.ValueObjects</c>.
        /// </summary>
        /// <remarks>
        /// The transformer adds the constraints enforced by each value object to its OpenAPI schema:
        /// <list type="bullet">
        ///   <item><description><c>Email</c>: string, <c>format: email</c>, pattern, maximum length 254.</description></item>
        ///   <item><description><c>Currency</c>: string, pattern, minimum and maximum length 3.</description></item>
        ///   <item><description><c>SSN</c>: string, pattern, minimum and maximum length 10.</description></item>
        ///   <item><description><c>IBAN</c>: string, <c>format: iban</c>, pattern, minimum length 13 and maximum length 34.</description></item>
        ///   <item><description><c>Money</c>: object description and example.</description></item>
        /// </list>
        /// <see cref="AddDefaultsDocumentTransformer"/> registers this transformer automatically.
        /// </remarks>
        /// <returns>The configured <see cref="OpenApiOptions"/>.</returns>
        public OpenApiOptions AddValueObjectSchemaTransformer() =>
            options.AddSchemaTransformer<ValueObjectSchemaTransformer>();

        /// <summary>
        /// Adds schema transformers that incorporate FluentValidation rules into the OpenAPI document.
        /// </summary>
        /// <returns>The configured <see cref="OpenApiOptions"/>.</returns>
        public OpenApiOptions AddFluentValidationRules() =>
            options.AddSchemaTransformer<FluentValidationSchemaTransformer>();

        /// <summary>
        /// Adds a schema transformer that enriches enum types in the OpenAPI schema with XML documentation descriptions.
        /// </summary>
        /// <param name="xmlPath">The path to the XML documentation file containing enum descriptions.</param>
        /// <returns>The configured <see cref="OpenApiOptions"/>.</returns>
        public OpenApiOptions AddEnumXmlDescriptionTransformer(string xmlPath) =>
            options.AddSchemaTransformer(new EnumXmlSchemaTransformer(xmlPath));

        /// <summary>
        /// Adds the <see cref="DefaultResponsesOperationTransformer"/> to the OpenAPI options.
        /// This transformer adds default error responses to all operations.
        /// </summary>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddDefaultResponsesOperationTransformer() =>
            options.AddOperationTransformer<DefaultResponsesOperationTransformer>();

        /// <summary>
        /// Adds the <see cref="DocumentInfoTransformer"/> to the OpenAPI options.
        /// This transformer populates the document's Info object from configuration settings.
        /// </summary>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddDocumentInfoTransformer() =>
            options.AddDocumentTransformer<DocumentInfoTransformer>();

        /// <summary>
        /// Adds the <see cref="BearerSecurityDocumentTransformer"/> to the OpenAPI options.
        /// This transformer adds a Bearer security scheme if JWT authentication is detected.
        /// </summary>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddBearerSecurityDocumentTransformer() =>
            options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();

        /// <summary>
        /// Adds the <see cref="HeadersParameterTransformer"/> to the OpenAPI options.
        /// This transformer adds configured request and response headers to all operations.
        /// </summary>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddHeadersParameterTransformer() =>
            options.AddDocumentTransformer<HeadersParameterTransformer>();

        /// <summary>
        /// Adds the <see cref="PolymorphismDocumentTransformer"/> to the OpenAPI options.
        /// This filter supports polymorphic schemas by adding derived types and configuring a discriminator.
        /// </summary>
        /// <param name="typesToInclude"></param>
        /// <param name="discriminatorPropertyName"></param>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddPolymorphismDocumentTransformer<T>(Dictionary<string, Type> typesToInclude, string discriminatorPropertyName = "Type") =>
            options.AddDocumentTransformer(new PolymorphismDocumentTransformer(typeof(T).Name, typesToInclude, discriminatorPropertyName));

        /// <summary>
        /// Adds the <see cref="TagOperationTransformer"/> to the OpenAPI options.
        /// This transformer adds or updates tag descriptions in the OpenAPI document.
        /// </summary>
        /// <param name="tags">A dictionary where the key is the tag name and the value is the description.</param>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddTagOperationTransformer(IDictionary<string, string> tags) =>
            options.AddDocumentTransformer(new TagOperationTransformer(tags));

        /// <summary>
        /// Adds the <see cref="HealthChecksDocumentTransformer"/> to the OpenAPI options.
        /// This transformer configures health check endpoints and their associated tags.
        /// </summary>
        /// <param name="tags">The tags to associate with health check endpoints.</param>
        /// <returns>The configured <see cref="OpenApiOptions"/> for chaining.</returns>
        public OpenApiOptions AddHealthChecksDocumentTransformer(IDictionary<string, string> tags) =>
            options.AddDocumentTransformer<HealthChecksDocumentTransformer>();
    }
}
