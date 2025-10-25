using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.DocumentTransformers;

/// <summary>
/// Transforms the OpenAPI document to include a Bearer security scheme if a `Bearer` or `BearerToken`
/// authentication scheme is registered in the application.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private const string BearerSchemeName = "Bearer";

    /// <summary>
    /// Examines registered authentication schemes and adds a Bearer security scheme to the
    /// OpenAPI document if a "Bearer" or "BearerToken" scheme is found.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The context for the document transformation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> that completes when the transformation is finished.</returns>
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IAuthenticationSchemeProvider? authenticationSchemeProvider =
            context?.ApplicationServices.GetService<IAuthenticationSchemeProvider>();

        if (authenticationSchemeProvider is null)
        {
            return;
        }

        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name is BearerSchemeName or "BearerToken"))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            // Add the security scheme if it doesn't already exist.
            if (document.Components.SecuritySchemes.TryAdd(BearerSchemeName, DefaultSecurityScheme()))
            {
                // Apply the security requirement to all operations.
                foreach (OpenApiOperation? operation in document.Paths.Values.SelectMany(path => path?.Operations?.Values))
                {
                    if (operation is not null)
                    {
                        operation.Security ??= [];
                        operation.Security.Add(new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecuritySchemeReference(BearerSchemeName)] = []
                        });
                    }
                }
            }
        }
    }

    private OpenApiSecurityScheme DefaultSecurityScheme()
    {
        return new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = BearerSchemeName,
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'"
        };
    }
}
