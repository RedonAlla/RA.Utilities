---
title: DocumentInfoTransformer
sidebar_position: 1
---

```powershell
Namespace: RA.Utilities.OpenApi.DocumentTransformers
```

The `DocumentInfoTransformer` is an `IOpenApiDocumentTransformer` that populates the top-level information of your OpenAPI document (like title, version, and description) directly from your `appsettings.json` configuration.

### 🎯 Purpose

The `DocumentInfoTransformer` is a utility that automates the population of your OpenAPI (Swagger) document's main information section.
Its primary purpose is to decouple your API's metadata—like its title, version, description, and contact information—from your source code.

Instead of hard-coding these details in your `Program.cs` file, this transformer reads them directly from your `appsettings.json` configuration.

#### 🔑 Key Benefits:

1. **Configuration over Code**: It allows you to change your API's documented title, version, or contact email without needing to recompile and redeploy your application.
2. **Environment-Specific Documentation**: You can easily have different descriptions or contact points for your Development, Staging, and Production environments using environment-specific `appsettings.json` files.
3. **Clean `Program.cs`**: It keeps your startup code cleaner by moving static configuration data to the appropriate configuration files.

In essence, it's a simple but effective tool for making your API documentation more flexible and maintainable.

### ⚙️ How It Works

1.  **Reads from Configuration**: It uses the [`OpenApiInfoSettings`](../Configuration/OpenApiInfoSettings.md) class to bind to a configuration section (by default, `OpenApiInfoSettings`).
2.  **Populates `OpenApiInfo`**: It sets the `Title`, `Version`, `Description`, `Summary`, `TermsOfService`, `Contact`, and `License` fields in the generated OpenAPI document.
3.  **Safe Updates**: It only updates fields if a corresponding value is found in the configuration, so it won't overwrite any values you may have set programmatically elsewhere.

:::info[v10.1.1]
`DocumentInfoTransformer` started populating `document.Info.Summary` from `OpenApiInfoSettings.Summary` in v10.1.1. Earlier versions ignored the `Summary` setting.
:::

### 🚀 Usage

The recommended way to use this transformer is by registering the default set of transformers with the `AddDefaultsDocumentTransformer()` extension method, which includes the `DocumentInfoTransformer` automatically.

#### Step 1: Configure Info in `appsettings.json`

Define the information you want to display in your `appsettings.json`.

```json showLineNumbers
// appsettings.json
{
  "OpenApiInfoSettings": {
    "Title": "My Awesome API",
    "Version": "v1.0",
    "Summary": "A brief summary shown next to the title.",
    "Description": "An API to demonstrate OpenAPI transformers.",
    "Contact": {
      "Name": "Support Team",
      "Email": "support@example.com"
    }
  }
}
```

#### Step 2: Register Services in `Program.cs`

Configure the settings and add the default transformers.

```csharp showLineNumbers
// Program.cs
using RA.Utilities.OpenApi.Extensions;
// highlight-next-line
using RA.Utilities.OpenApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// highlight-start
// Bind the settings from appsettings.json
builder.Services.Configure<OpenApiInfoSettings>(
    builder.Configuration.GetSection(OpenApiInfoSettings.AppSettingsKey)
);

// Add the OpenApi services and register the default transformers
builder.Services.AddOpenApi(options =>
{
    options.AddDefaultsDocumentTransformer();
});
// highlight-end

var app = builder.Build();
// ...
app.MapOpenApi();
```
