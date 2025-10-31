```powershell
Namespace: RA.Utilities.Feature.Extensions
```

The `FeatureBuilderExtensions` class is the primary entry point for configuring the `RA.Utilities.Feature` library within your application.

Its main purpose is to provide a **fluent and discoverable API for registering your CQRS features** such as commands, queries, notifications, and their handlers—with the .NET dependency injection ([`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection)) container.

This class uses a combination of extension methods and the **Builder** pattern to create a clean, readable, and chainable configuration experience.

## 🛠️ How It Works
Let's look at the methods in the class to understand the pattern.


```csharp showLineNumbers
// From: /RA.Utilities/Application/RA.Utilities.Feature/Extensions/FeatureBuilderExtensions.cs

public static class FeatureBuilderExtensions
{
    // For requests with a response (e.g., queries)
    public static FeatureBuilder<TRequest, TResponse> AddFeature<TRequest, TResponse, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        // 1. Registers the core handler for the request.
        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
        
        // 2. Returns a builder object for chaining.
        return new FeatureBuilder<TRequest, TResponse>(services);
    }

    // For requests without a response (e.g., commands)
    public static FeatureBuilder<TRequest> AddFeature<TRequest, THandler>(
        this IServiceCollection services)
        where TRequest : IRequest
        where THandler : class, IRequestHandler<TRequest>
    {
        services.AddScoped<IRequestHandler<TRequest>, THandler>();
        return new FeatureBuilder<TRequest>(services);
    }

    // For notifications (events)
    public static NotificationFeatureBuilder<TNotification> AddNotification<TNotification>(
        this IServiceCollection services)
        where TNotification : INotification
    {
        return new NotificationFeatureBuilder<TNotification>(services);
    }
}
```

## 🔑 Key Design Points
### 1. Extension Methods as Entry Points:
The methods extend [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection), allowing you to start the configuration process directly on the services collection in your `Program.cs`, which is a standard and intuitive pattern in .NET.

### 2. Builder Pattern for Fluent Chaining:
The most important design choice here is that the methods **return a builder object** (`FeatureBuilder` or `NotificationFeatureBuilder`).
This builder holds a reference to the [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) and exposes its own methods for adding related components, like validators or pipeline behaviors.

### 3. Encouraging Vertical Slice Configuration:
This design allows you to define an entire vertical slice in a single, coherent block of code. As seen in the fallowing example, this creates a highly readable configuration:

**Example**
```csharp showLineNumbers
// Example of the fluent API in action
_ = services
      // 1. Starts with the extension method, registers the handler.
      .AddFeature<CreateProductCommand, CreateProductHandler>() 
      // 2. Chains a behavior using the returned FeatureBuilder.
      .AddDecoration<LoggingBehavior<CreateProductCommand>>()   
      // 3. Chains a validator using the FeatureBuilder.
      .AddValidator<CreateProductCommandValidator>();
```

## 🧠 Summary
In summary, `FeatureBuilderExtensions` acts as the "front door" to the `RA.Utilities.Feature` library's setup.
It abstracts away the raw [`IServiceCollection`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection) registration details behind a purpose-built, fluent API that improves code readability and helps developers correctly wire up their application features.
