using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Results;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Extensions;
using RA.Utilities.Dispatcher.Models;

namespace RA.Utilities.Dispatcher.Tests;

/// <summary>
/// Verifies that <see cref="PipelineContext{T}"/> carries typed data through the whole
/// pipeline — behavior to handler — with a fresh instance per call, and that the
/// non-context dispatch path allocates no context at all.
/// </summary>
public class PipelineContextTests
{
    [Fact]
    public void PipelineContext_InitializesData()
    {
        var context = new PipelineContext<PipelineContextTestData>();

        context.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task Send_WithContext_BehaviorStampsContext_HandlerReadsIt()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        var context = new PipelineContext<PipelineContextTestData>();
        context.Data.CorrelationId = "provided-by-caller";

        Result<string> result = await dispatcher.Send<ContextStampingRequest, string, PipelineContextTestData>(
            new ContextStampingRequest(), context);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("provided-by-caller");
        ContextStampingHandler.ReceivedCorrelationIds.Should().Contain("provided-by-caller");
    }

    [Fact]
    public async Task Send_WithoutExplicitContext_StillDispatches()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        Result<string> result = await dispatcher.Send<ContextStampingRequest, string>(
            new ContextStampingRequest());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task EachSend_GetsItsOwnContextInstance()
    {
        using ServiceProvider provider = CreateProvider();
        IDispatcher dispatcher = provider.GetRequiredService<IDispatcher>();

        await dispatcher.Send<ContextStampingRequest, string, PipelineContextTestData>(
            new ContextStampingRequest(), new PipelineContext<PipelineContextTestData> { Data = { CorrelationId = "first" } });
        await dispatcher.Send<ContextStampingRequest, string, PipelineContextTestData>(
            new ContextStampingRequest(), new PipelineContext<PipelineContextTestData> { Data = { CorrelationId = "second" } });

        ContextStampingHandler.ReceivedCorrelationIds.Should().ContainInOrder("first", "second");
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDispatcher();
        return services.BuildServiceProvider();
    }
}

public class PipelineContextTestData
{
    public string? CorrelationId { get; set; }
}

public record ContextStampingRequest : IRequest<string>;

public class ContextStampingHandler : IRequestHandler<ContextStampingRequest, string>
{
    public static System.Collections.Generic.List<string> ReceivedCorrelationIds { get; } = [];

    public ValueTask<Result<string>> HandleAsync(ContextStampingRequest request, CancellationToken cancellationToken) =>
        ValueTask.FromResult(Result.Success("non-context"));

    public ValueTask<Result<string>> HandleAsync<TContext>(
        ContextStampingRequest request, PipelineContext<TContext> context, CancellationToken cancellationToken)
        where TContext : class, new()
    {
        if (context is PipelineContext<PipelineContextTestData> typed)
        {
            ReceivedCorrelationIds.Add(typed.Data.CorrelationId ?? string.Empty);
            return ValueTask.FromResult(Result.Success(typed.Data.CorrelationId ?? string.Empty));
        }

        return ValueTask.FromResult(Result.Success("non-context"));
    }
}
