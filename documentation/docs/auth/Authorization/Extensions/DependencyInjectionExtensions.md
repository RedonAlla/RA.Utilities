---
title: DependencyInjectionExtensions
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `DependencyInjectionExtensions` class provides a convenient extension method to simplify the registration of the `ICurrentUser` service in your application's dependency injection container.

### 🎯 Purpose

The `DependencyInjectionExtensions` class provides a convenient shortcut for registering the services related to the `AppUser` utility.
Its purpose is to simplify the setup process in `Program.cs`.

Instead of requiring developers to know that `AppUser` depends on [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor) and registering both manually, this class provides a single extension method, `AddCurrentUser()`, that handles all the necessary registrations.

This approach:

1. **Reduces Boilerplate**: It turns a multi-line setup into a single, declarative call.
2. **Encapsulates Implementation Details**: It hides the fact that `AppUser` needs [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor), making the setup cleaner and less prone to error.
3. **Promotes Best Practices**: By registering the service, it encourages developers to depend on abstractions, not concrete implementations, which is crucial for testability and maintainability.

## 🧩 Available Extensions

### AddCurrentUser()

Registers `AppUser` as the transient. It also registers [`IHttpContextAccessor`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.ihttpcontextaccessor), which is required by `AppUser` to access the current request's user claims.

#### Usage

Call `AddCurrentUser()` in your `Program.cs` file when configuring your services.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// highlight-next-line
builder.Services.AddCurrentUser();

// ... other service registrations
```

After registration, you can inject `AppUser` into your controllers and services to access information about the authenticated user.
