using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Handlers;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies that the abstract <see cref="RequestHandler{TRequest, TResponse}"/> /
/// <see cref="RequestHandler{TRequest}"/> base classes are pure passthroughs: the explicit
/// interface implementations forward to the public HandleAsync override, and the protected
/// context-aware overload delegates to the plain one by default.
/// </summary>
public class RequestHandlerTests
{
    [Fact]
    public async Task RequestResponse_HandleAsync_ReturnsResultFromOverride()
    {
        var handler = new PlainResponseHandler();

        Result<string> result = await handler.HandleAsync(new PlainResponseRequest("ping"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task RequestResponse_InterfaceDispatch_ForwardsToOverride()
    {
        IRequestHandler<PlainResponseRequest, string> handler = new PlainResponseHandler();

        Result<string> result = await handler.HandleAsync(new PlainResponseRequest("ping"), CancellationToken.None);

        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task ContextOverload_DelegatesToPlainHandleAsync_ByDefault()
    {
        IRequestHandler<PlainResponseRequest, string> handler = new PlainResponseHandler();

        Result<string> result = await handler.HandleAsync(
            new PlainResponseRequest("ping"),
            new PipelineContext<PipelineTestContext>(),
            CancellationToken.None);

        result.Value.Should().Be("pong:ping");
    }

    [Fact]
    public async Task ContextOverload_WhenOverridden_ConsumesContext()
    {
        IRequestHandler<ContextForwardRequest, string> handler = new ContextForwardHandler();
        var context = new PipelineContext<PipelineTestContext>();
        context.Data.CorrelationId = "abc-123";

        await handler.HandleAsync(new ContextForwardRequest(), context, CancellationToken.None);

        ContextForwardHandler.ReceivedCorrelationIds.Should().Contain("abc-123");
    }

    [Fact]
    public async Task Void_HandleAsync_ReturnsSuccess()
    {
        var handler = new VoidPassthroughHandler();

        Result result = await handler.HandleAsync(new VoidPassthroughRequest(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        VoidPassthroughHandler.ReceivedCount.Should().Be(1);
    }
}

public class PipelineTestContext
{
    public string? CorrelationId { get; set; }
}

public record PlainResponseRequest(string Value) : IRequest<string>;

public class PlainResponseHandler : RequestHandler<PlainResponseRequest, string>
{
    public override ValueTask<Result<string>> HandleAsync(PlainResponseRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success($"pong:{request.Value}"));
}

public record ContextForwardRequest : IRequest<string>;

public class ContextForwardHandler : RequestHandler<ContextForwardRequest, string>
{
    public static List<string> ReceivedCorrelationIds { get; } = [];

    public override ValueTask<Result<string>> HandleAsync(ContextForwardRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success("ok"));

    protected override ValueTask<Result<string>> HandleAsync<TContext>(
        ContextForwardRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
    {
        if (context is PipelineContext<PipelineTestContext> typed)
            ReceivedCorrelationIds.Add(typed.Data.CorrelationId ?? string.Empty);

        return base.HandleAsync(request, context, cancellationToken);
    }
}

public record VoidPassthroughRequest : IRequest;

public class VoidPassthroughHandler : RequestHandler<VoidPassthroughRequest>
{
    public static int ReceivedCount { get; set; }

    public override ValueTask<Result> HandleAsync(VoidPassthroughRequest request, CancellationToken cancellationToken)
    {
        ReceivedCount++;
        return ValueTask.FromResult(Result.Success());
    }
}
