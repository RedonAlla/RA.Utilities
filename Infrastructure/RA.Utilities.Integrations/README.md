<p align="center">
  <img src="../..//Assets/Images/http.png" alt="RA.Utilities.Integrations Logo" width="128">
</p>

# RA.Utilities.Integrations

[![NuGet version](https://img.shields.io/nuget/v/RA.Utilities.Integrations.svg)](https://www.nuget.org/packages/RA.Utilities.Integrations/)

A utility library to simplify and standardize HTTP client calls in .NET applications. It builds upon `IHttpClientFactory` to provide a structured way to configure and use typed HTTP clients for various integrations.

## 🎯 Purpose

Integrating with external APIs is a common requirement. This package provides a robust and repeatable pattern for managing these integrations by:

- **Standardizing Configuration**: Centralizes HTTP client settings (like Base URL, timeouts, and default headers) in your `appsettings.json`.
- **Simplifying Registration**: Offers a single extension method to register a typed `HttpClient`, configure it from settings, and set up logging and retry policies.
- **Promoting Best Practices**: Encourages the use of typed `HttpClient`s, which provides better compile-time safety and intellisense.
- **Improving Resilience**: Includes built-in support for transient fault handling with Polly.

## ✨ Core Components

-   **`HttpClientSettings`**: A base class for creating strongly-typed configuration objects that map to your `appsettings.json`.
-   **`AddIntegrationHttpClient<TClient, TSettings>`**: An `IServiceCollection` extension method that wires everything up:
    -   Registers a typed `HttpClient` (`TClient`).
    -   Binds configuration from `appsettings.json` to your settings class (`TSettings`).
    -   Configures the `HttpClient`'s `BaseAddress`, `Timeout`, and default headers.
    -   Adds a default transient error handling policy (retry with exponential backoff).

## 🛠️ Installation

Install the package via the .NET CLI:

```bash
dotnet add package RA.Utilities.Integrations
```

Or through the NuGet Package Manager console:

```powershell
Install-Package RA.Utilities.Integrations
```

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

## 🔗 Dependencies

-   **RA.Utilities.Core.Constants**: Used for common constants.
-   **RA.Utilities.Logging.Shared**: Provides shared components for logging.
-   **Microsoft.Extensions.Http**: Core library for `IHttpClientFactory`.
-   **Microsoft.Extensions.Options.DataAnnotations**: For validating configuration settings.