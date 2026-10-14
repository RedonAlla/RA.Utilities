---
title: DependencyInjectionExtensions
sidebar_position: 3
---

```powershell
Namespace: RA.Utilities.Authorization.Extensions
```

The `DependencyInjectionExtensions` class provides a convenient extension method to register the `IUserContext` service.

### 🎯 Purpose

A single call to `AddUserContext()` registers `IUserContext` as scoped, backed by the internal `UserContext` implementation.
The implementation reads the current request's `ClaimsPrincipal` through `IHttpContextAccessor`, which this method does **not** register — call `builder.Services.AddHttpContextAccessor()` yourself.

## 🧩 Available Extensions

### AddUserContext()

Registers `IUserContext` as scoped (implemented by the internal `UserContext` class).

> **v10.0.2 change**: this method replaces `AddAppUser()` from earlier versions.
>
> **v10.0.3 change**: registers the `IUserContext` interface (`IUserContext` → `UserContext`). In v10.0.2 only the internal concrete `UserContext` class was registered, so injecting `IUserContext` failed. It also no longer calls `AddHttpContextAccessor()` — register `IHttpContextAccessor` explicitly. See the [migration guide](../migration-guides.mdx).

#### Usage

Register `IHttpContextAccessor` and `AddUserContext()` in your `Program.cs`:

```csharp showLineNumbers
// Program.cs
using RA.Utilities.Authorization.Extensions;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
builder.Services.AddHttpContextAccessor();
builder.Services.AddUserContext();
// highlight-end

// ... other service registrations
```

After registration, inject `IUserContext` into your controllers and services to access authenticated user information.
