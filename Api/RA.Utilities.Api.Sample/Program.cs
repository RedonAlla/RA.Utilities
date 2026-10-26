using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Exposes the endpoints through an OpenAPI document that Scalar renders.
builder.Services.AddOpenApi();
// Registers the services URL-segment API versioning needs: the apiVersion route constraint and the
// version reporter used by ReportApiVersions.
builder.Services.AddApiVersioning();

WebApplication app = builder.Build();

// One version set shared by every group and endpoint registered below.
ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

// Discovers every IEndpointGroup and IEndpoint implementation in this assembly at compile time:
// all groups are created first, then every endpoint is mapped into the group identified by its
// GroupName. The version set is forwarded to every MapGroup and MapEndpoint implementation, which
// apply it with WithApiVersionSet. There is no reflection, no DI scanning, and no manual
// registration list.
app.MapEndpoints(apiVersionSet);

// Interactive API reference at /scalar/v1, backed by the OpenAPI document at /openapi/v1.json.
app.MapOpenApi();
app.MapScalarApiReference();

await app.RunAsync();
