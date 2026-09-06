using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Api.Extensions;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Exposes the endpoints through an OpenAPI document that Scalar renders.
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Discovers every IEndpointGroup and IEndpoint implementation in this assembly at compile time:
// all groups are created first, then every endpoint is mapped into the group identified by its
// GroupName. There is no reflection, no DI scanning, and no manual registration list.
app.MapEndpoints();

// Interactive API reference at /scalar/v1, backed by the OpenAPI document at /openapi/v1.json.
app.MapOpenApi();
app.MapScalarApiReference();

await app.RunAsync();
