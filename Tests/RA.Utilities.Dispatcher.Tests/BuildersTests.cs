using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Behaviors;
using RA.Utilities.Dispatcher.Builders;
using RA.Utilities.Dispatcher.Extensions;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies the fluent builder registration surface: <c>AddDispatcher</c> /
/// <c>AddNotification</c> register their services with the documented lifetimes, chain
/// decorations and validators, and return the same builder instance for fluent composition.
/// </summary>
public class BuildersTests
{
    [Fact]
    public void AddDispatcher_RequestResponse_RegistersHandlerScoped_AndReturnsBuilder()
    {
        var services = new ServiceCollection();

        DispatcherBuilder<PlainResponseRequest, string> builder =
            services.AddDispatcher<PlainResponseRequest, string, PlainResponseHandler>();

        services.Should().ContainSingle(IsDescriptor<IRequestHandler<PlainResponseRequest, string>, PlainResponseHandler>(ServiceLifetime.Scoped));
        builder.Services.Should().BeSameAs(services);
    }

    [Fact]
    public void AddDispatcher_Void_RegistersHandlerScoped()
    {
        var services = new ServiceCollection();

        services.AddDispatcher<VoidPassthroughRequest, VoidPassthroughHandler>();

        services.Should().ContainSingle(IsDescriptor<IRequestHandler<VoidPassthroughRequest>, VoidPassthroughHandler>(ServiceLifetime.Scoped));
    }

    [Fact]
    public void AddDecoration_RegistersBehaviorTransient_AndChainsTheSameBuilder()
    {
        var services = new ServiceCollection();

        DispatcherBuilder<LoggingRequest, string> builder =
            services.AddDispatcher<LoggingRequest, string, LoggingHandler>();
        DispatcherBuilder<LoggingRequest, string> chained =
            builder.AddDecoration<LoggingBehavior<LoggingRequest, string>>();

        services.Should().ContainSingle(IsDescriptor<IPipelineBehavior<LoggingRequest, string>, LoggingBehavior<LoggingRequest, string>>(ServiceLifetime.Transient));
        chained.Should().BeSameAs(builder);
    }

    [Fact]
    public void AddValidator_RegistersValidatorAndValidationBehavior()
    {
        var services = new ServiceCollection();

        services
            .AddDispatcher<CreateOrderCommand, CreateOrderResult, CreateOrderHandler>()
            .AddValidator<CreateOrderValidator>();

        services.Should().ContainSingle(IsDescriptor<IValidator<CreateOrderCommand>, CreateOrderValidator>(ServiceLifetime.Transient));
        services.Should().ContainSingle(IsDescriptor<IPipelineBehavior<CreateOrderCommand, CreateOrderResult>, ValidationBehavior<CreateOrderCommand, CreateOrderResult>>(ServiceLifetime.Transient));
    }

    [Fact]
    public void AddNotification_AddHandler_AddDecoration_RegisterTransientServices()
    {
        var services = new ServiceCollection();

        services
            .AddNotification<FanOutNotification>()
            .AddHandler<FanOutHandlerA>()
            .AddDecoration<NotificationLoggingBehavior<FanOutNotification>>();

        services.Should().ContainSingle(IsDescriptor<INotificationHandler<FanOutNotification>, FanOutHandlerA>(ServiceLifetime.Transient));
        services.Should().ContainSingle(IsDescriptor<INotificationBehavior<FanOutNotification>, NotificationLoggingBehavior<FanOutNotification>>(ServiceLifetime.Transient));
    }

    private static System.Linq.Expressions.Expression<System.Func<ServiceDescriptor, bool>> IsDescriptor<TService, TImplementation>(ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService =>
        descriptor =>
            descriptor.ServiceType == typeof(TService)
            && descriptor.ImplementationType == typeof(TImplementation)
            && descriptor.Lifetime == lifetime;
}
