using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Abstractions;
using RA.Utilities.Feature.Models;
using Xunit;

namespace RA.Utilities.Tests.Generators;

/// <summary>
/// Loads a generated <c>MediatorImpl</c> from an in-memory compilation (compiled against the real
/// RA.Utilities.Feature runtime, with the module-initializer test seam enabled) and verifies its
/// dispatch behavior against a real DI container.
/// </summary>
public class MediatorImplBehaviorTests
{
    private const string Sources = """
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;

        namespace Sample
        {
            public record Ping(string Value) : global::RA.Utilities.Feature.Abstractions.IRequest<Pong>;

            public record Pong(string Value);

            public class PingHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<Ping, Pong>
            {
                public Task<Pong> HandleAsync(Ping request, CancellationToken cancellationToken) => Task.FromResult(new Pong("pong:" + request.Value));
            }

            public record CountRequest : global::RA.Utilities.Feature.Abstractions.IRequest<int>;

            public class CountHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<CountRequest, int>
            {
                public Task<int> HandleAsync(CountRequest request, CancellationToken cancellationToken) => Task.FromResult(42);
            }

            public struct StructRequest : global::RA.Utilities.Feature.Abstractions.IRequest<int>;

            public class StructHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<StructRequest, int>
            {
                public Task<int> HandleAsync(StructRequest request, CancellationToken cancellationToken) => Task.FromResult(7);
            }

            public record DoNothing : global::RA.Utilities.Feature.Abstractions.IRequest;

            public class DoNothingHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<DoNothing>
            {
                public Task HandleAsync(DoNothing request, CancellationToken cancellationToken) => Task.CompletedTask;
            }

            public record ThrowingRequest : global::RA.Utilities.Feature.Abstractions.IRequest<string>;

            public class ThrowingHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<ThrowingRequest, string>
            {
                public Task<string> HandleAsync(ThrowingRequest request, CancellationToken cancellationToken) => throw new InvalidOperationException("handler failure");
            }

            public record OrderPlaced(string OrderId) : global::RA.Utilities.Feature.Abstractions.INotification;

            public class FirstNotifier : global::RA.Utilities.Feature.Abstractions.INotificationHandler<OrderPlaced>
            {
                public static List<string> Received = [];

                public Task HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
                {
                    Received.Add("first:" + notification.OrderId);
                    return Task.CompletedTask;
                }
            }

            public class FaultyNotifier : global::RA.Utilities.Feature.Abstractions.INotificationHandler<OrderPlaced>
            {
                public Task HandleAsync(OrderPlaced notification, CancellationToken cancellationToken) => throw new InvalidOperationException("boom");
            }

            public class SecondNotifier : global::RA.Utilities.Feature.Abstractions.INotificationHandler<OrderPlaced>
            {
                public Task HandleAsync(OrderPlaced notification, CancellationToken cancellationToken)
                {
                    FirstNotifier.Received.Add("second:" + notification.OrderId);
                    return Task.CompletedTask;
                }
            }

            public class TracingBehavior : global::RA.Utilities.Feature.Abstractions.IPipelineBehavior<Ping, Pong>
            {
                public static List<string> Trace = [];

                public Task<Pong> HandleAsync(Ping request, global::RA.Utilities.Feature.Models.RequestHandlerDelegate<Pong> next, CancellationToken cancellationToken)
                {
                    Trace.Add("before");
                    Task<Pong> task = next();
                    Trace.Add("after");
                    return task;
                }
            }

            public record ContextRequest : global::RA.Utilities.Feature.Abstractions.IRequest<string>;

            public class ContextData
            {
                public string? Tag { get; set; }
            }

            public class ContextWritingBehavior : global::RA.Utilities.Feature.Abstractions.IPipelineBehavior<ContextRequest, string>
            {
                public Task<string> HandleAsync(ContextRequest request, global::RA.Utilities.Feature.Models.RequestHandlerDelegate<string> next, CancellationToken cancellationToken) => next();

                public Task<string> HandleAsync<TContext>(ContextRequest request, global::RA.Utilities.Feature.Models.RequestHandlerContextDelegate<string, TContext> next, global::RA.Utilities.Feature.Models.PipelineContext<TContext> context, CancellationToken cancellationToken)
                    where TContext : class, new()
                {
                    if (context.Data is ContextData data)
                    {
                        data.Tag = "written-by-behavior";
                    }

                    return next(context);
                }
            }

            public record DoubleBehaviorRequest : global::RA.Utilities.Feature.Abstractions.IRequest<string>;

            public class DoubleBehaviorHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<DoubleBehaviorRequest, string>
            {
                public Task<string> HandleAsync(DoubleBehaviorRequest request, CancellationToken cancellationToken) => Task.FromResult("done");
            }

            public class AlphaBehavior : global::RA.Utilities.Feature.Abstractions.IPipelineBehavior<DoubleBehaviorRequest, string>
            {
                public static List<string> Trace = [];

                public Task<string> HandleAsync(DoubleBehaviorRequest request, global::RA.Utilities.Feature.Models.RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
                {
                    Trace.Add("alpha-before");
                    Task<string> task = next();
                    Trace.Add("alpha-after");
                    return task;
                }
            }

            public class BetaBehavior : global::RA.Utilities.Feature.Abstractions.IPipelineBehavior<DoubleBehaviorRequest, string>
            {
                public Task<string> HandleAsync(DoubleBehaviorRequest request, global::RA.Utilities.Feature.Models.RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
                {
                    AlphaBehavior.Trace.Add("beta-before");
                    Task<string> task = next();
                    AlphaBehavior.Trace.Add("beta-after");
                    return task;
                }
            }

            public class ContextReadingHandler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<ContextRequest, string>
            {
                public Task<string> HandleAsync(ContextRequest request, CancellationToken cancellationToken) => Task.FromResult("no-context");

                public Task<string> HandleAsync<TContext>(ContextRequest request, global::RA.Utilities.Feature.Models.PipelineContext<TContext> context, CancellationToken cancellationToken)
                    where TContext : class, new()
                    => Task.FromResult(((ContextData)(object)context.Data).Tag ?? "missing");
            }
        }
        """;

    private sealed class Host : IDisposable
    {
        public Host(ServiceProvider provider, Assembly assembly)
        {
            Provider = provider;
            Assembly = assembly;
        }

        public ServiceProvider Provider { get; }

        public Assembly Assembly { get; }

        public void Dispose() => Provider.Dispose();
    }

    [Fact]
    public async Task Send_ResponseRequest_ReturnsResponse()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object? pong = await InvokeTypedSend(mediator, NewPing(host), host.Assembly.GetType("Sample.Pong")!);

        GetProperty(pong!, "Value").Should().Be("pong:hello");
    }

    [Fact]
    public async Task Send_Object_DispatchesByRuntimeType()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object? result = await SendObject(mediator, NewPing(host));

        result.Should().BeOfType(host.Assembly.GetType("Sample.Pong"));
    }

    [Fact]
    public async Task Send_ValueTypeResponse_Object_ReturnsBoxedValue()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object countRequest = Activator.CreateInstance(host.Assembly.GetType("Sample.CountRequest")!)!;
        object? result = await SendObject(mediator, countRequest);

        result.Should().Be(42);
    }

    [Fact]
    public async Task Send_StructMessage_Object_ReturnsBoxedValue()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object structRequest = Activator.CreateInstance(host.Assembly.GetType("Sample.StructRequest")!)!;
        object? result = await SendObject(mediator, structRequest);

        result.Should().Be(7);
    }

    [Fact]
    public async Task Send_VoidRequest_Completes()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object doNothing = Activator.CreateInstance(host.Assembly.GetType("Sample.DoNothing")!)!;

        Func<Task> act = () => mediator.Send((dynamic)doNothing);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Send_BehaviorsWrapHandlerInRegistrationOrder()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        await InvokeTypedSend(mediator, NewPing(host), host.Assembly.GetType("Sample.Pong")!);

        GetStaticList<string>(host, "Sample.TracingBehavior", "Trace").Should().Equal("before", "after");
    }

    [Fact]
    public async Task Send_MultipleBehaviors_ComposeInDeclarationOrder_WithoutRecursion()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object request = Activator.CreateInstance(host.Assembly.GetType("Sample.DoubleBehaviorRequest")!)!;
        MethodInfo send = GetGenericSendMethod(2);
        MethodInfo closedSend = send.MakeGenericMethod(request.GetType(), typeof(string));

        Func<Task> act = () => (Task)closedSend.Invoke(mediator, new object?[] { request, CancellationToken.None })!;

        await act.Should().NotThrowAsync();

        // AlphaBehavior sorts first, so it is the outermost behavior.
        GetStaticList<string>(host, "Sample.AlphaBehavior", "Trace")
            .Should().Equal("alpha-before", "beta-before", "beta-after", "alpha-after");
    }

    [Fact]
    public async Task Send_WithContext_FlowsContextThroughBehaviorAndHandler()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        Type contextDataType = host.Assembly.GetType("Sample.ContextData")!;
        Type contextRequestType = host.Assembly.GetType("Sample.ContextRequest")!;
        object contextRequest = Activator.CreateInstance(contextRequestType)!;
        object pipelineContext = Activator.CreateInstance(typeof(PipelineContext<>).MakeGenericType(contextDataType))!;

        MethodInfo send = typeof(IMediator).GetMethods().Single(static method =>
            method.Name == "Send" && method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 3);
        MethodInfo closedSend = send.MakeGenericMethod(contextRequestType, typeof(string), contextDataType);

        object?[] arguments = [contextRequest, pipelineContext, CancellationToken.None];
        Task<string> task = (Task<string>)closedSend.Invoke(mediator, arguments)!;

        (await task).Should().Be("written-by-behavior");
    }

    [Fact]
    public async Task Send_PropagatesHandlerExceptions()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object request = Activator.CreateInstance(host.Assembly.GetType("Sample.ThrowingRequest")!)!;
        MethodInfo send = GetGenericSendMethod(2);
        MethodInfo closedSend = send.MakeGenericMethod(request.GetType(), typeof(string));

        object?[] arguments = [request, CancellationToken.None];
        Func<Task> act = () => (Task)closedSend.Invoke(mediator, arguments)!;

        // The handler throws synchronously, so the reflection layer wraps it.
        (await act.Should().ThrowAsync<TargetInvocationException>())
            .Which.InnerException.Should().BeOfType<InvalidOperationException>()
            .Which.Message.Should().Be("handler failure");
    }

    [Fact]
    public async Task Publish_FansOutToAllHandlers_AndSwallowsFailures()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object notification = Activator.CreateInstance(host.Assembly.GetType("Sample.OrderPlaced")!, ["order-1"])!;
        MethodInfo publish = GetGenericPublishMethod();
        MethodInfo closedPublish = publish.MakeGenericMethod(notification.GetType());

        object?[] arguments = new object?[] { notification, CancellationToken.None };
        Func<Task> act = () => (Task)closedPublish.Invoke(mediator, arguments)!;

        await act.Should().NotThrowAsync();
        GetStaticList<string>(host, "Sample.FirstNotifier", "Received")
            .Should().BeEquivalentTo("first:order-1", "second:order-1");
    }

    [Fact]
    public async Task Publish_Object_FansOutByRuntimeType()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object notification = Activator.CreateInstance(host.Assembly.GetType("Sample.OrderPlaced")!, ["order-2"])!;
        MethodInfo publishObject = mediator.GetType().GetMethod("Publish", [typeof(object), typeof(CancellationToken)])!;

        object?[] arguments = new object?[] { notification, CancellationToken.None };
        Func<Task> act = () => (Task)publishObject.Invoke(mediator, arguments)!;

        await act.Should().NotThrowAsync();
        GetStaticList<string>(host, "Sample.FirstNotifier", "Received")
            .Should().BeEquivalentTo("first:order-2", "second:order-2");
    }

    [Fact]
    public async Task Send_Object_NullRequest_ThrowsArgumentNullException()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        Func<Task> act = async () => await SendObject(mediator, null!);

        (await act.Should().ThrowAsync<TargetInvocationException>())
            .WithInnerException<ArgumentNullException>();
    }

    [Fact]
    public async Task Send_UnknownMessage_IsResolvedThroughTheGenericInterfaceBody()
    {
        using Host host = CreateHost();

        // The request type is declared in this test assembly, which the loaded generated mediator
        // does not know; the generic interface method resolves it from DI directly.
        string response = await host.Provider.GetRequiredService<IMediator>()
            .Send<UnknownToGeneratedRequest, string>(new UnknownToGeneratedRequest());

        response.Should().Be("fallback-ok");
    }

    [Fact]
    public async Task Send_Object_UnknownType_ThrowsHandlerNotFoundException()
    {
        using Host host = CreateHost();
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        Func<Task> act = async () => await SendObject(mediator, new object());

        (await act.Should().ThrowAsync<TargetInvocationException>())
            .WithInnerException<global::RA.Utilities.Feature.Exceptions.HandlerNotFoundException>();
    }

    [Fact]
    public async Task Send_LargeProject_DispatchesThroughDictionaries()
    {
        // 17 messages exceed the dictionary threshold (8), so the generated mediator switches to
        // fast dictionary dispatch; every 5th message has a value-type response, exercising the
        // boxing and unboxing helpers of the dictionary path.
        using Host host = CreateHost(BuildLargeSources(17));
        IMediator mediator = host.Provider.GetRequiredService<IMediator>();

        object request1 = Activator.CreateInstance(host.Assembly.GetType("Sample.LargeRequest1")!)!;
        (await InvokeTypedSend(mediator, request1, typeof(string))).Should().Be("ok");
        (await SendObject(mediator, request1)).Should().Be("ok");

        object request5 = Activator.CreateInstance(host.Assembly.GetType("Sample.LargeRequest5")!)!;
        (await SendObject(mediator, request5)).Should().Be(1);
    }

    private static string BuildLargeSources(int count)
    {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine("using System.Threading;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("namespace Sample");
        builder.AppendLine("{");

        for (int i = 0; i < count; i++)
        {
            bool valueTypeResponse = i % 5 == 0;
            string responseType = valueTypeResponse ? "int" : "string";
            string responseValue = valueTypeResponse ? "1" : "\"ok\"";

            builder.AppendLine($"    public record LargeRequest{i} : global::RA.Utilities.Feature.Abstractions.IRequest<{responseType}>;");
            builder.AppendLine($"    public class LargeRequest{i}Handler : global::RA.Utilities.Feature.Abstractions.IRequestHandler<LargeRequest{i}, {responseType}>");
            builder.AppendLine("    {");
            builder.AppendLine($"        public Task<{responseType}> HandleAsync(LargeRequest{i} request, CancellationToken cancellationToken) => Task.FromResult({responseValue});");
            builder.AppendLine("    }");
        }

        builder.AppendLine("}");
        return builder.ToString();
    }

    private static Host CreateHost(string sources = Sources)
    {
        Assembly assembly = GeneratorTestHost.CompileAndLoadMediator([sources]);
        Type mediatorImplType = assembly.GetType("RA.Utilities.Feature.Generated.MediatorImpl")!;

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped(typeof(IMediator), mediatorImplType);
        services.AddScoped<IRequestHandler<UnknownToGeneratedRequest, string>, UnknownToGeneratedHandler>();

        // Mirror the generated registrations: handlers and behaviors under their interfaces plus
        // handler self-registrations (the generated mediator injects handlers by concrete type).
        foreach (Type handlerType in assembly.GetTypes().OrderBy(static type => type.Name))
        {
            if (handlerType.IsAbstract || handlerType.IsInterface)
            {
                continue;
            }

            bool isPipelineType = false;
            foreach (Type @interface in handlerType.GetInterfaces())
            {
                if (@interface.IsGenericType
                    && (@interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                        || @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
                        || @interface.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                        || @interface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)
                        || @interface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<>)))
                {
                    services.AddScoped(@interface, handlerType);
                    isPipelineType = true;
                }
            }

            if (isPipelineType)
            {
                services.AddScoped(handlerType);
            }
        }

        return new Host(services.BuildServiceProvider(), assembly);
    }

    private static async Task<object?> InvokeTypedSend(IMediator mediator, object request, Type responseType)
    {
        MethodInfo closedSend = GetGenericSendMethod(2).MakeGenericMethod(request.GetType(), responseType);

        object?[] arguments = [request, CancellationToken.None];
        Task task = (Task)closedSend.Invoke(mediator, arguments)!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task);
    }

    private static MethodInfo GetGenericSendMethod(int genericArgumentCount) =>
        typeof(IMediator).GetMethods().Single(method =>
            method.Name == "Send"
            && method.IsGenericMethodDefinition
            && method.GetGenericArguments().Length == genericArgumentCount
            && method.GetParameters().Length == 2);

    private static MethodInfo GetGenericPublishMethod() =>
        typeof(IMediator).GetMethods().Single(method =>
            method.Name == "Publish"
            && method.IsGenericMethodDefinition
            && method.GetParameters().Length == 2);

    private static async Task<object?> SendObject(IMediator mediator, object request)
    {
        MethodInfo method = mediator.GetType().GetMethod("Send", [typeof(object), typeof(CancellationToken)])!;
        object?[] arguments = new object?[] { request, CancellationToken.None };
        // The generated reference-type-response path reinterprets the Task<TResponse> without
        // changing its runtime type, so unwrap through the non-generic Task.
        Task task = (Task)method.Invoke(mediator, arguments)!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task);
    }

    private static object NewPing(Host host) =>
        Activator.CreateInstance(host.Assembly.GetType("Sample.Ping")!, ["hello"])!;

    private static object? GetProperty(object instance, string name) =>
        instance.GetType().GetProperty(name)!.GetValue(instance);

    private static List<T> GetStaticList<T>(Host host, string typeName, string fieldName)
    {
        FieldInfo field = host.Assembly.GetType(typeName)!
            .GetField(fieldName, BindingFlags.Public | BindingFlags.Static)!;

        return (List<T>)field.GetValue(null)!;
    }
}

public record UnknownToGeneratedRequest : IRequest<string>;

public class UnknownToGeneratedHandler : IRequestHandler<UnknownToGeneratedRequest, string>
{
    public Task<string> HandleAsync(UnknownToGeneratedRequest request, CancellationToken cancellationToken) =>
        Task.FromResult("fallback-ok");
}
