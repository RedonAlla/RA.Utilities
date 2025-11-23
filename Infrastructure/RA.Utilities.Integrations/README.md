# RA.Utilities.Integrations

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Integrations.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/)
[![Codecov](https://codecov.io/github/RedonAlla/RA.Utilities/graph/badge.svg)](https://codecov.io/github/RedonAlla/RA.Utilities)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RA.Utilities.Integrations.svg?logo=nuget)](https://www.nuget.org/packages/RA.Utilities.Integrations/)
[![Documentation](https://img.shields.io/badge/Documentation-read-brightgreen.svg?logo=readthedocs&logoColor=fff)](https://redonalla.github.io/RA.Utilities/nuget-packages/Integrations/)
[![GitHub license](https://img.shields.io/github/license/RedonAlla/RA.Utilities?logo=googledocs&logoColor=fff)](https://github.com/RedonAlla/RA.Utilities?tab=MIT-1-ov-file)


`RA.Utilities.Integrations` is a utility library that simplifies and standardizes HTTP client calls in .NET applications. It solves the problem of managing external API integrations by providing a robust and repeatable pattern that centralizes configuration, simplifies registration, and improves resilience with built-in retry policies.

By building on `IHttpClientFactory`, it promotes best practices like using typed `HttpClient`s, which provides better compile-time safety and intellisense.

## Getting started

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Integrations
```

Or through the NuGet Package Manager console:

```powershell
Install-Package RA.Utilities.Integrations
```

## 🔗 Dependencies

-   [`RA.Utilities.Core.Constants`](https://redonalla.github.io/RA.Utilities/nuget-packages/core/RA.Utilities.Core.Constants/)
-   [`RA.Utilities.Logging.Shared`](https://redonalla.github.io/RA.Utilities/nuget-packages/Logging/RA.Utilities.Logging.Shared/)
-   [`Microsoft.AspNetCore.Http`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http)
-   [`Microsoft.Extensions.DependencyInjection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection)
-   [`Microsoft.Extensions.Http`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.http)
-   [`Microsoft.Extensions.Logging.Abstractions`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.abstractions)
-   [`Microsoft.Extensions.Options.ConfigurationExtensions`](https://www.nuget.org/packages/microsoft.extensions.options.configurationextensions/)
-   [`Microsoft.Extensions.Options.DataAnnotations`](https://www.nuget.org/packages/Microsoft.Extensions.Options.DataAnnotations)

## Additional documentation

For more information on how this package fits into the larger RA.Utilities ecosystem, please see the main repository [documentation](https://redonalla.github.io/RA.Utilities/nuget-packages/Integrations/).

---

## 🚀 Usage Guide

Here’s a step-by-step guide to setting up a typed client for an external API.

### 1. Add Configuration to `appsettings.json`

Define the settings for your integration. The section name (e.g., `"MyApi"`) will be used later for binding.

```json
{
  "Integrations": {
    "MyApi": {
      "BaseUrl": "https://api.example.com/v1/",
      "TimeoutSeconds": 60,
      "DefaultHeaders": {
        "X-Api-Key": "your-secret-api-key",
        "Accept": "application/json"
      }
    }
  }
}
```

### 2. Create a Settings Class

Create a class that inherits from `HttpClientSettings` to represent your configuration.

```csharp
using RA.Utilities.Integrations.Options;

public class MyApiSettings : HttpClientSettings
{
    // You can add custom properties specific to this integration if needed.
}
```

### 3. Create a Typed `HttpClient`

This is the client your application will use to communicate with the API. It takes `HttpClient` in its constructor, which will be configured and provided by the `IHttpClientFactory`.

```csharp
using System.Text.Json;

public interface IMyApiClient
{
    Task<Product?> GetProductAsync(int id);
}

public class MyApiClient : IMyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyApiClient> _logger;

    public MyApiClient(HttpClient httpClient, ILogger<MyApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve product with ID {ProductId}.", id);
            return null;
        }
    }
}

// Example DTO
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### 4. Register the Client in `Program.cs`

Use the `AddIntegrationHttpClient` extension method to register your typed client and bind its settings.

```csharp
using RA.Utilities.Integrations.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register the typed client for "MyApi"
builder.Services.AddIntegrationHttpClient<IMyApiClient, MyApiClient, MyApiSettings>(
    builder.Configuration,
    configSection: "Integrations:MyApi"
);

// ... other services

var app = builder.Build();
```

### 5. Use the Client in Your Services

Inject your typed client interface (`IMyApiClient`) wherever you need to call the external API.

```csharp
public class ProductService
{
    private readonly IMyApiClient _myApiClient;

    public ProductService(IMyApiClient myApiClient)
    {
        _myApiClient = myApiClient;
    }

    public async Task<Product?> GetProductDetails(int productId)
    {
        return await _myApiClient.GetProductAsync(productId);
    }
}
```