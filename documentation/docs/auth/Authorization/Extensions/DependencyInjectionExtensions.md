---
title: DependencyInjectionExtensions
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `DependencyInjectionExtensions` class provides a convenient extension method to register `IUserContext` and its dependencies.

### 🎯 Purpose

Instead of manually registering `IUserContext` and `IHttpContextAccessor`, a single call to `AddUserContext()` handles all necessary registrations.
This reduces boilerplate, encapsulates implementation details, and promotes best practices.

## 🧩 Available Extensions

### AddUserContext()

Registers `IUserContext` as scoped and adds `IHttpContextAccessor` (required to access the current request's user claims).

> **v10.0.2 change**: this method replaces `AddAppUser()` from earlier versions.

#### Usage

Call `AddUserContext()` in your `Program.cs`:

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-next-line
builder.Services.AddUserContext();

// ... other service registrations
```

After registration, inject `IUserContext` into your controllers and services to access authenticated user information.
