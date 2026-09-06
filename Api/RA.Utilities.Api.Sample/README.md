# RA.Utilities.Api.Sample

A minimal API sample demonstrating compile-time endpoint registration with
`RA.Utilities.Api`.

## Run

```bash
dotnet run --project Api/RA.Utilities.Api.Sample/RA.Utilities.Api.Sample.csproj
```

Try the routes:

| Method | Route            | Description        |
| ------ | ---------------- | ------------------ |
| GET    | `/api/todos`     | List all todos     |
| POST   | `/api/todos`     | Create a todo      |
| GET    | `/api/users`     | List all users     |
| GET    | `/api/users/{id:int}` | Get a user by id |
| GET    | `/scalar/v1`     | Interactive API reference (Scalar UI) |

```mermaid
---
references:
  - "File: /Api/RA.Utilities.Api.Generators/Diagnostics.cs"
  - "File: /Api/RA.Utilities.Api.Generators/EndpointRegistrationGenerator.cs"
  - "File: /Api/RA.Utilities.Api.Generators/EquatableArray.cs"
  - "File: /Api/RA.Utilities.Api.Generators/KnownMetadataNames.cs"
  - "File: /Api/RA.Utilities.Api.Generators/Models/EndpointModels.cs"
  - "File: /Api/RA.Utilities.Api.Generators/SourceEmitter.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/CreateTodoEndpoint.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/GetTodosEndpoint.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/GetUserByIdEndpoint.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/GetUsersEndpoint.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/TodosGroup.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Endpoints/UsersGroup.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Models/Todo.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Models/User.cs"
  - "File: /Api/RA.Utilities.Api.Sample/Program.cs"
  - "File: /Api/RA.Utilities.Api/Abstractions/IEndpoint.cs"
  - "File: /Api/RA.Utilities.Api/Abstractions/IEndpointGroup.cs"
  - "File:
    /Api/RA.Utilities.Api/Extensions/HttpEndpointServiceCollectionExtensions.cs"
  - "File: /Api/RA.Utilities.Api/Extensions/MiddlewareExtensions.cs"
  - "File:
    /Tests/RA.Utilities.Api.Tests/Generators/EndpointRegistrationGeneratorTests\
    .cs"
  - "File: /Tests/RA.Utilities.Api.Tests/Generators/GeneratorTestHost.cs"
  - "File: /Tests/RA.Utilities.Api.Tests/Runtime/EndpointRegistrationTests.cs"
  - "File: /Tests/RA.Utilities.Api.Tests/Runtime/TestEndpoints.cs"
  - "File:
    /Tests/RA.Utilities.Tests/RA.Utilities.Api/Extensions/RaExceptionHandlingEx\
    tensionsTests.cs"
generationTime: 2026-09-06T15:33:07.891Z
---
architecture-beta
    group generator(cloud)[Source Generator Layer]
    group abstractions(cloud)[Abstractions Layer]
    group runtime(cloud)[Runtime Layer]
    group sample(cloud)[Sample Application]

    service endpointRegGen(server)[Endpoint Registration Generator] in generator
    service diagnostics(server)[Diagnostics] in generator
    service sourceEmitter(server)[Source Emitter] in generator
    service models(disk)[Endpoint Models] in generator
    service knownNames(disk)[Known Metadata Names] in generator
    service equatableArray(disk)[Equatable Array] in generator

    service iendpointGroup(server)[IEndpointGroup Interface] in abstractions
    service iendpoint(server)[IEndpoint Interface] in abstractions

    service httpExtensions(server)[HttpEndpoint Service Extensions] in runtime
    service middlewareExtensions(server)[Middleware Extensions] in runtime

    service todosGroup(server)[Todos Group] in sample
    service usersGroup(server)[Users Group] in sample
    service todoEndpoints(server)[Todo Endpoints] in sample
    service userEndpoints(server)[User Endpoints] in sample
    service program(server)[Program Bootstrap] in sample
    service todoModel(disk)[Todo Model] in sample
    service userModel(disk)[User Model] in sample

    endpointRegGen:R --> L:diagnostics
    endpointRegGen:R --> L:sourceEmitter
    endpointRegGen:R --> L:models
    endpointRegGen:R --> L:knownNames

    sourceEmitter:R --> L:equatableArray
    sourceEmitter:R --> L:knownNames

    models:B --> T:knownNames

    endpointRegGen{group}:R --> L:iendpointGroup{group}
    endpointRegGen{group}:R --> L:iendpoint{group}

    iendpointGroup{group}:R --> L:httpExtensions{group}
    iendpoint{group}:R --> L:httpExtensions{group}

    httpExtensions{group}:R --> L:program{group}
    middlewareExtensions{group}:R --> L:program{group}

    todosGroup:B --> T:iendpointGroup{group}
    usersGroup:B --> T:iendpointGroup{group}
    todoEndpoints:B --> T:iendpoint{group}
    userEndpoints:B --> T:iendpoint{group}

    program:L --> R:todosGroup
    program:L --> R:usersGroup
    program:L --> R:todoEndpoints
    program:L --> R:userEndpoints

    todoEndpoints:B --> T:todoModel
    userEndpoints:B --> T:userModel
```

## How it works

The whole `Program.cs` is:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapEndpoints();
app.Run();
```

Everything else is discovered at **compile time** by the
`RA.Utilities.Api.Generators` source generator (shipped inside the `RA.Utilities.Api`
NuGet package):

* `Endpoints/TodosGroup.cs` and `Endpoints/UsersGroup.cs` implement `IEndpointGroup`
  and build the shared `RouteGroupBuilder` (prefix + tags) for their feature.
* `Endpoints/GetTodosEndpoint.cs`, `Endpoints/CreateTodoEndpoint.cs`,
  `Endpoints/GetUsersEndpoint.cs`, and `Endpoints/GetUserByIdEndpoint.cs` implement
  `IEndpoint` and map their routes into the group identified by their `GroupName`.

The generator validates the group names at compile time: a duplicate `GroupName`
(`EPMG001`) or an endpoint referencing a missing group (`EPMG002`) is a build error,
not a runtime surprise.
