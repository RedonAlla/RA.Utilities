---
title: BearerSecurityDocumentTransformer
sidebar_position: 2
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `BearerSecurityDocumentTransformer` is an `IOpenApiDocumentTransformer` that automatically adds a "Bearer" security scheme to your OpenAPI document. This enables the "Authorize" button in the Swagger UI, allowing users to easily test protected endpoints.

### 🎯 Purpose

The `BearerSecurityDocumentTransformer` is a smart utility that automates the process of documenting JWT Bearer authentication in your OpenAPI (Swagger) specification.

In a typical ASP.NET Core application that uses JWTs for security, you want your Swagger UI to have an "Authorize" button.
This button allows developers to enter a JWT token, which is then automatically included in the `Authorization` header for subsequent API calls made from the UI.

Manually configuring this involves adding boilerplate code to your `Program.cs` to define the security scheme and apply it to your endpoints.
The `BearerSecurityDocumentTransformer` eliminates this manual work.

### ⚙️ How It Works

1. **Automatic Detection**: It inspects the authentication schemes registered in your application's dependency injection container.
2. **Conditional Logic**: If it finds an authentication scheme named `"Bearer"` or `"BearerToken"`, it concludes that your application uses JWT Bearer authentication.
3. **Documentation Generation**: It then programmatically modifies the generated OpenAPI document to:
  * Add the standard "Bearer" security scheme definition. This is what creates the ***"Authorize"*** button and the input field for the token in the Swagger UI.
  * Apply this security requirement to every single API operation. This adds the lock icon next to each endpoint, indicating that it is protected.
By using this transformer, your API documentation stays in sync with your authentication setup automatically, reducing boilerplate and preventing configuration mistakes.

This ensures your API documentation accurately reflects your security setup without any extra code in your endpoint definitions.

### 🚀 Usage

The recommended way to use this transformer is by registering the default set of transformers with the `AddDefaultsDocumentTransformer()` extension method.
This method includes the `BearerSecurityDocumentTransformer` automatically.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Assumes JWT Bearer authentication is already configured, e.g.:
// builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddEndpointsApiExplorer();

// highlight-start
builder.Services.AddOpenApi()
    .AddDefaultsDocumentTransformer();
// highlight-end

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpenApi();

// ... your endpoints
```

With this setup, the Swagger UI will automatically display an "Authorize" button, allowing developers to authenticate and test protected endpoints.
