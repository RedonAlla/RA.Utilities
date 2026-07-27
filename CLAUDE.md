# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build the entire solution
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Run a single test project
dotnet test Tests/RA.Utilities.Tests/RA.Utilities.Tests.csproj

# Run tests with a filter (class or method name)
dotnet test --filter "FullyQualifiedName~ResultTests"
```

Treat warnings as errors is enabled solution-wide via `Directory.Build.props`. Always build after changes to verify no warnings are introduced.

## Architecture Overview

This is a .NET 10.0 solution producing ~17 NuGet packages that provide reusable building blocks for web API development. It enforces **Vertical Slice Architecture with CQRS** and follows the **Dependency Rule**: dependencies point inward (Api → Application → Core).

### Layer Map

| Layer | Projects | Purpose |
|---|---|---|
| **Core** | `RA.Utilities.Core`, `.Core.Constants`, `.Core.Exceptions` | Zero-dependency foundation: `Result` monad, exception hierarchy, HTTP constants |
| **Application** | `RA.Utilities.Feature`, `RA.Utilities.Application.Validation` | Custom mediator, CQRS handlers, pipeline behaviors, FluentValidation utilities |
| **Api** | `.Api`, `.Api.Results`, `.Api.Middlewares`, `.OpenApi`, `.Authentication.JwtBearer`, `.Authorization` | ASP.NET Core middleware, endpoint registration, typed HTTP responses, OpenAPI transformers, JWT auth |
| **Data** | `.Data.Entities`, `.Data.Abstractions`, `.Data.EntityFramework` | Entity base classes, repository interfaces (read/write/composite), EF Core implementations |
| **Infrastructure** | `RA.Utilities.Integrations` | Typed HTTP client with delegating-handler pipeline (logging, auth, proxy, header forwarding) |
| **Logging** | `.Logging.Core`, `.Logging.Shared` | Serilog setup with request-id enrichment, HTTP log templates |

### Key Architectural Patterns

**Result Monad** (`RA.Utilities.Core.Results`): Operations return `Result` or `Result<T>` instead of throwing. Chain with `Map`, `Bind`, `Match`, `OnSuccess`, `OnFailure` (sync and async overloads). Implicit conversions from `Exception` (failure) and `T` (success).

**Custom Mediator** (`RA.Utilities.Feature`): Not MediatR. Register features with `services.AddFeature<TRequest, TResponse, THandler>()` which returns a builder for chaining `.AddValidator<T>()` and `.AddDecoration<T>()`. The mediator composes pipeline behaviors via `Aggregate` + `Reverse()`, wrapping the handler as the innermost delegate. Handlers return `Result<T>` — convert exceptions to `Result.Failure` via the implicit conversion. Call `services.AddMediator()` once at startup.

**Exception Hierarchy** (`RA.Utilities.Core.Exceptions`): `RaBaseException` with `ResponseCode` (HTTP status) and `ErrorCode` (string). Concrete types: `NotFoundException`, `ConflictException`, `BadRequestException` (with `ValidationErrors[]`), `UnauthorizedException`, `ForbiddenException`, `UnprocessableException`.

**Exception-to-Response Mapping**: `ErrorResultResponse.Result(Exception)` dispatches typed exceptions to the correct `*Response` type. `GlobalExceptionHandler` (implements `IExceptionHandler`) logs and calls this mapper. The `SuccessResponse` static class provides `Ok()`, `Created()`, `Accepted()`, `NoContent()` helpers wrapping payloads in `SuccessResponse<T>`.

**DI Registration Convention**: Every project has extension methods on `IServiceCollection` (or `OpenApiOptions` for OpenAPI). Lifetimes are documented inline in their respective classes. Endpoints are auto-discovered via `AddEndpoints(assembly)` + `MapEndpoints(app)` scanning for `IEndpoint` implementors.

**Entity Framework**: Repository pattern with separate read/write concerns. `RepositoryBase<T>` composes `ReadRepositoryBase<T>` and `WriteRepositoryBase<T>` rather than inheriting both. Use `AddRepositoryBase()` / `AddReadRepositoryBase()` / `AddWriteRepositoryBase()` to register open-generic DI. `BaseEntitySaveChangesInterceptor` auto-sets `CreatedAt`/`LastModifiedAt`.

## NuGet Publishing

Publishing is manual via the `Publish NuGet Package` GitHub Actions workflow. Each project carries its own `<Version>` in its `.csproj`. To publish, update the version in the `.csproj`, merge to `main`, then trigger the workflow with the package name (e.g., `RA.Utilities.Api`). The workflow extracts the version from the `.csproj` and publishes to both NuGet.org and GitHub Packages, then creates a git tag in `{PackageName}/{Version}` format.

Central package management is enabled — all dependency versions live in `Directory.Packages.props`.

## Tests

- Framework: **xUnit** + **FluentAssertions**
- Coverage: **coverlet** + Codecov upload in CI
- Tests are only present for `Core` and `Core.Exceptions` currently
- Test project references source projects directly (not as NuGet packages)
