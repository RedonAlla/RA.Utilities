---
title: DependencyInjectionExtensions
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `DependencyInjectionExtensions` class provides a convenient extension method to register `AppUser` and its dependencies.

### 🎯 Purpose

Instead of manually registering `AppUser` and `IHttpContextAccessor`, a single call to `AddAppUser()` handles all necessary registrations. This reduces boilerplate, encapsulates implementation details, and promotes best practices.

## 🧩 Available Extensions

### AddAppUser()

Registers `AppUser` as transient and adds `IHttpContextAccessor` (required by `AppUser` to access the current request's user claims).

#### Usage

Call `AddAppUser()` in your `Program.cs`:

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddAppUser();

// ... other service registrations
```

After registration, inject `AppUser` into your controllers and services to access authenticated user information.
