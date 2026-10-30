using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Exceptions;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Models;
using GeneratedDispatcher = RA.Utilities.Dispatcher.Generated.Dispatcher;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Locks in the Result-based contract of the generated dispatcher: successes and expected
/// failures come back as <see cref="Result"/> values, truly exceptional cases propagate,
/// and object-based dispatch maps runtime types with a typed failure for unknown messages.
/// </summary>
public class DispatcherTests
{
    [Fact]
    public async Task Send_ReturnsSuccessResult_WithHandlerValue()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<SuccessRequest, string>(new SuccessRequest("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task Send_ReturnsFailureResult_WithHandlerException_InsteadOfThrowing()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<FailureRequest, string>(new FailureRequest());

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<NotFoundException>();
    }

    [Fact]
    public async Task Send_Void_ReturnsSuccessResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result result = await dispatcher.Send(new VoidSuccessRequest());

        result.IsSuccess.Should().BeTrue();
        VoidSuccessHandler.ReceivedCount.Should().Be(1);
    }

    [Fact]
    public async Task Send_PropagatesTrulyExceptionalHandlerExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Func<Task> act = () => dispatcher.Send<ThrowingRequest, string>(new ThrowingRequest()).AsTask();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Send_PropagatesBehaviorExceptions()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Func<Task> act = () => dispatcher.Send<BehaviorThrowRequest, string>(new BehaviorThrowRequest()).AsTask();

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Send_WithContext_ReturnsSuccessResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        var context = new PipelineContext<DispatchTestContext>();

        Result<string> result = await dispatcher.Send<SuccessRequest, string, DispatchTestContext>(
            new SuccessRequest("ping"), context);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task Send_WithContext_ReturnsFailureResult()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<FailureRequest, string, DispatchTestContext>(
            new FailureRequest(), new PipelineContext<DispatchTestContext>());

        result.IsFailure.Should().BeTrue();
        result.Exception.Should().BeOfType<NotFoundException>();
    }

    [Fact]
    public async Task ObjectSend_KnownMessage_DispatchesByRuntimeType()
    {
        using ServiceProvider provider = CreateProvider();
        GeneratedDispatcher dispatcher = provider.GetRequiredService<GeneratedDispatcher>();

        Result<object?> result = await dispatcher.Send((object)new SuccessRequest("ping"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task ObjectSend_UnknownMessage_ThrowsHandlerNotFoundException()
    {
        using ServiceProvider provider = CreateProvider();
        GeneratedDispatcher dispatcher = provider.GetRequiredService<GeneratedDispatcher>();

        Func<Task> act = () => dispatcher.Send((object)new NotAMessage()).AsTask();

        HandlerNotFoundException exception = (await act.Should().ThrowAsync<HandlerNotFoundException>()).Which;
        exception.MessageType.Should().Be<NotAMessage>();
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher();
        return services.BuildServiceProvider();
    }
}

public class DispatchTestContext
{
    public string? CorrelationId { get; set; }
}

public record SuccessRequest(string Value) : IRequest<string>;

public class SuccessHandler : IRequestHandler<SuccessRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(SuccessRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success($"pong:{request.Value}"));
}

public record FailureRequest : IRequest<string>;

public class FailureHandler : IRequestHandler<FailureRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(FailureRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Failure<string>(new NotFoundException("FailureRequest", "none")));
}

public record VoidSuccessRequest : IRequest;

public class VoidSuccessHandler : IRequestHandler<VoidSuccessRequest>
{
    public static int ReceivedCount { get; set; }

    public ValueTask<Result> HandleAsync(VoidSuccessRequest request, CancellationToken cancellationToken)
    {
        ReceivedCount++;
        return ValueTask.FromResult(Result.Success());
    }
}

public record ThrowingRequest : IRequest<string>;

public class ThrowingHandler : IRequestHandler<ThrowingRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(ThrowingRequest request, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("truly exceptional");
}

public record BehaviorThrowRequest : IRequest<string>;

public class BehaviorThrowHandler : IRequestHandler<BehaviorThrowRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(BehaviorThrowRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success("ok"));
}

/// <summary>
/// Closed behavior so the source generator auto-registers it; sending the request always
/// runs through this behavior and its exception propagates.
/// </summary>
public class ThrowingClosedBehavior : IPipelineBehavior<BehaviorThrowRequest, string>
{
    public ValueTask<Result<string>> HandleAsync(
        BehaviorThrowRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        throw new ForbiddenException();
}

public record NotAMessage;
