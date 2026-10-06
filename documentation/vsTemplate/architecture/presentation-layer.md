---
title: Presentation Layer
sidebar_position: 7
---

# Presentation Layer

The Presentation layer is the outermost tier and the system's gateway: it is the **only layer that knows about HTTP**.
It translates incoming requests into Application-layer messages, returns HTTP responses, and acts as the **composition root** that wires every other layer together at startup. It lives in `src/Presentation/` and is split into two projects.

| Project | Type | Responsibility |
| :--- | :--- | :--- |
| `RaTemplate.Api` | ASP.NET Core Web (`Microsoft.NET.Sdk.Web`) | The host: `Program.cs`, endpoints, middleware, OpenAPI, auth, health checks |
| `RaTemplate.Api.Contracts` | Class library | Shared request/response contract objects exposed over the wire |

`RaTemplate.Api` references Application, Infrastructure, and Api.Contracts — as the outermost layer it alone references every inner component, while dependencies otherwise flow strictly inward.

## Composition Root (Program.cs)

`Program.cs` builds the host in a deliberate order: configure logging, register services, build, initialize, then assemble the request pipeline.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

builder.AddLoggingWithConfiguration();

builder.Services
    .Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddDefaultHealthChecks()
    .AddOpenApi(configuration)
    .AddRaExceptionHandling()
    .AddProblemDetails();
//#if (UseAuthorization)
builder.Services.AddAuthorization(configuration);
//#endif
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(configuration)
    .AddDefaultMiddlewares();

WebApplication app = builder.Build();

//#if (UseAnyDatabase)
if (app.Environment.IsDevelopment())
    await RaTemplateDbInitializer.InitializeDatabaseAsync(app.Services);
//#endif

if (!app.Environment.IsProduction())
    app.UseOpenApi();

app.MapHealthCheckEndpoints()
   .UseDefaultMiddlewares();
//#if (UseAuthorization)
app.UseAuth();
//#endif
app.MapEndpoints();

await app.RunAsync();
```

Service registration cascades outward-in: `AddApplicationServices()` (Application) and `AddInfrastructureServices(configuration)` (Infrastructure, which forwards to Persistence and Integration). JSON enums serialize as strings by default via `JsonStringEnumConverter`.

## Endpoints

Routing is built on the `RA.Utilities.Api` package's **source-generated endpoint pipeline**. Two abstractions drive it:

- **`IEndpointGroup`** — declares a shared `RouteGroupBuilder` (prefix, tags, versioning, conventions) via a static `GroupName` and `MapGroup(IEndpointRouteBuilder)`. Mapped once, before its endpoints.
- **`IEndpoint`** — declares a static `GroupName` that matches a group, plus `MapEndpoint(RouteGroupBuilder)` defining one or more routes.

A source generator discovers every implementation at compile time and emits the `MapEndpoints()` extension called at the end of `Program.cs` — there is no runtime assembly scanning and no manual route registration. The template ships with no endpoints; you add your own, typically under an `Endpoints/` folder:

```csharp
using Microsoft.AspNetCore.Routing;
using RA.Utilities.Api.Abstractions;
using RA.Utilities.Feature.Abstractions;

namespace RaTemplate.Api.Endpoints.Customers;

public sealed class CustomersGroup : IEndpointGroup
{
    public static string GroupName => "Customers";
    public static RouteGroupBuilder MapGroup(IEndpointRouteBuilder app) =>
        app.MapGroup("/api/customers").WithTags(GroupName);
}

public sealed class GetCustomerEndpoint : IEndpoint
{
    public static string GroupName => CustomersGroup.GroupName;

    public static void MapEndpoint(RouteGroupBuilder group) =>
        group.MapGet("/{id}", GetAsync);

    public static async Task<Ok<CustomerResponse>> GetAsync(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        CustomerOutput output = await mediator.Send(new GetCustomerInput(id), cancellationToken);
        return TypedResults.Ok(new CustomerResponse(output.Id, output.Name, output.Email));
    }
}
```

Endpoints only dispatch to the Application layer via `IMediator.Send(...)` and shape the HTTP response — they contain **no domain rules**.

### Endpoint conventions (enforced)

`ApiTests` in `tests/RaTemplate.ArchitectureTests/` enforces two rules for every `IEndpoint`:

1. **Depends on the mediator** — the endpoint must reference `RA.Utilities.Feature.Abstractions`, i.e. it dispatches through `IMediator` rather than calling services directly.
2. **Returns a typed result wrapping an Api.Contracts object** — the response type (unwrapping `Task<>`/`ValueTask<>`) must be an `IResult` that contains a concrete class from the `RaTemplate.Api.Contracts` assembly. The only exceptions are results whose type name contains `NoContent` or `Redirect`.

Rule 2 is why there are two DTO tiers: Application handlers return `*Output` objects, and endpoints **map those to contract objects** in `RaTemplate.Api.Contracts` before returning them. This keeps the wire format decoupled from the Application layer's internal shapes.

## API Contracts

`RaTemplate.Api.Contracts` holds the request/response objects exposed over HTTP. It has **no project dependencies** (only the `RA.Utilities.Core.ValueObjects` package), so clients can reference the contracts without pulling in the API, Application, or Infrastructure. Endpoints must return these contract types (see conventions above).

## Middleware & Cross-Cutting Concerns

`AddDefaultMiddlewares()` / `UseDefaultMiddlewares()` (in `ServiceConfiguration/MiddlewareExtensions.cs`) register and apply the standard pipeline from `RA.Utilities.Api`:

- **Default headers middleware** — applies standard response headers.
- **Logging middleware** — logs requests/responses, truncating bodies at 16 KB and escalating to `Warning` above a duration threshold.
- **HTTPS redirection** and the **exception handler**.

Both middlewares ignore `/openapi-ui`, `/openapi`, and `/health` to keep docs and probes out of the request logs.

## Error Handling

`AddRaExceptionHandling()` registers a global exception handler and `AddProblemDetails()` enables RFC 9110 problem-details responses. Together they map the **typed exceptions** thrown by handlers and validators (see the [Application layer](application-layer.md)) to consistent HTTP responses — for example `BadRequestException` (carrying FluentValidation errors) → `400`, `NotFoundException` → `404`, `ConflictException` → `409` — so handlers never build HTTP results themselves.

## OpenAPI & Documentation UI

`AddOpenApi(configuration)` (in `ServiceConfiguration/OpenApiExtensions.cs`) binds `OpenApiInfoSettings` from configuration and, via `RA.Utilities.OpenApi`, applies default document transformers that populate the document info, add a **Bearer** security scheme for JWT, and add a standard `x-request-id` header parameter.

`UseOpenApi()` (called when not in Production) maps the OpenAPI document and the selected UI:

- **Scalar** (`OpenApiUI=scalar`, the default) — interactive UI at **`/openapi-ui`**, with a Bearer security scheme when authorization is enabled.
- **Swagger** (`OpenApiUI=swagger`) — Swagger UI against `/openapi/v1.json`, persisting the entered token across reloads.

## Authentication & Authorization

When `UseAuthorization` is `true`, `AddAuthorization(configuration)` (in `ServiceConfiguration/AuthorizationExtensions.cs`) registers the user context and JWT Bearer authentication (`AddUserContext()` + `AddJwtBearerAuthentication(configuration)`), and `app.UseAuth()` adds the middleware to the pipeline. This is the place to register authorization policies. The whole block is removed when authorization is disabled.

## Health Checks

`AddDefaultHealthChecks()` registers a `self` liveness check; Persistence adds per-provider database checks. `MapHealthCheckEndpoints()` exposes:

- **`/health`** — full health report (UI-formatted response writer).
- **`/alive`** — liveness probe filtered to the `live` tag.

## Running the API

From `launchSettings.json`:

| Profile | URL |
| :--- | :--- |
| `http` | `http://localhost:5039` |
| `https` | `https://localhost:7202` (and `http://localhost:5039`) |

```bash
dotnet run --project src/Presentation/RaTemplate.Api/RaTemplate.Api.csproj
```

Then open `http://localhost:5039/openapi-ui` for the interactive docs. A `RaTemplate.Api.http` file is included for making requests from your editor.

## Conditional Generation

The API project adapts to the parameters chosen at creation:

- **`UseAuthorization`** → the JWT packages, `AddAuthorization`, and `UseAuth` are included or removed.
- **`OpenApiUI`** → `Scalar.AspNetCore` + the Scalar UI block, or `Swashbuckle.AspNetCore` + the Swagger UI block.
- **A database selected** → `AspNetCore.HealthChecks.UI.Client`, the DB health-check UI writer, and the dev-time database initializer are included.

## Dependencies

The Presentation layer depends on Domain, Application, Infrastructure, and Api.Contracts.
As the outermost boundary and composition root, it is the only layer that references everything beneath it; no inner layer references it.