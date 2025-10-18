using System.Threading;
using System.Threading.Tasks;
using RA.Utilities.OpenApi.Settings;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// Transforms the OpenAPI document by populating the Info object with values from configuration.
/// </summary>
public sealed class DocumentInfoTransformer : IOpenApiDocumentTransformer
{
    private readonly OpenApiInfoSettings? _openApiInfoSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentInfoTransformer"/> class.
    /// </summary>
    /// <param name="openApiInfoSettings">The options containing Open API information settings.</param>
    public DocumentInfoTransformer(IOptions<OpenApiInfoSettings> openApiInfoSettings)
    {
        _openApiInfoSettings = openApiInfoSettings?.Value;
    }

    /// <summary>
    /// Transforms the <see cref="OpenApiDocument"/> by populating its <see cref="OpenApiInfo"/>
    /// section with values from the <see cref="OpenApiInfoSettings"/>.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the transformation is finished.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = _openApiInfoSettings?.Title ?? document.Info.Title;
        document.Info.Version = _openApiInfoSettings?.Version ?? document.Info.Version;
        document.Info.Description = _openApiInfoSettings?.Description ?? document.Info.Description;
        document.Info.TermsOfService = _openApiInfoSettings?.TermsOfService ?? document.Info.TermsOfService;
        //document.Info.Summary = _openApiInfoSettings?.Summary ?? document.Info.Summary;
        if (_openApiInfoSettings?.Contact is not null)
        {

            document.Info.Contact ??= new OpenApiContact();
            document.Info.Contact.Name = _openApiInfoSettings?.Contact.Name ?? document.Info.Contact.Name;
            document.Info.Contact.Email = _openApiInfoSettings?.Contact.Email ?? document.Info.Contact.Email;
            document.Info.Contact.Url = _openApiInfoSettings?.Contact.Url ?? document.Info.Contact.Url;
        }
        if (_openApiInfoSettings?.License is not null)
        {
            document.Info.License ??= new OpenApiLicense();
            document.Info.License.Name = _openApiInfoSettings?.License.Name ?? document.Info.License.Name;
            //document.Info.License.Identifier = _openApiInfoSettings?.License.Identifier ?? document.Info.License.Identifier;
            document.Info.License.Url = _openApiInfoSettings?.License.Url ?? document.Info.License.Url;
        }

        return Task.CompletedTask;
    }

}
