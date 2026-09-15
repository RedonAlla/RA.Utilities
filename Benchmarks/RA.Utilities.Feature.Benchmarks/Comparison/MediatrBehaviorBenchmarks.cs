using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Feature.Benchmarks.Shared;

namespace RA.Utilities.Feature.Benchmarks.Comparison;

/// <summary>
/// Benchmarks the MediatR mirror of the package's request pipeline behaviors, scenario for
/// scenario with <c>RequestBehaviorBenchmarks</c>: logging, validation (passing and failing),
/// logging + validation, and the void logging + validation pipeline. Behaviors and validators
/// are the MediatR counterparts defined in <c>MediatrBehaviorBehaviors</c>; handlers are scoped
/// and payloads identical to the Feature side. The failure scenario measures the validation
/// failure path (throw + catch inside the benchmark, so the run itself stays green).
/// </summary>
[MemoryDiagnoser]
public class MediatrBehaviorBenchmarks
{
    private readonly MediatRPingZeroRequest _barePing = new("payload");
    private readonly MediatRLoggingPingRequest _loggingPing = new("payload");
    private readonly MediatRValidatedPingRequest _validationPingValid = new("payload");
    private readonly MediatRValidatedPingRequest _validationPingInvalid = new(string.Empty);
    private readonly MediatRLoggingValidatedPingRequest _loggingValidationPing = new("payload");
    private readonly MediatRVoidBehaviorCommand _voidCommand = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(MediatrServices.ConfigureRequestBehaviors);
    }

    [Benchmark]
    public async Task Send_NoBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_barePing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Logging()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_loggingPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Validation_Passing()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_validationPingValid, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Validation_Failing()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        try
        {
            await mediator.Send(_validationPingInvalid, CancellationToken.None);
        }
        catch (ValidationException ex)
        {
            // Expected: the validation behavior throws before the handler runs. Catching here
            // keeps the benchmark measuring the failure path instead of failing the run.
            _ = ex;
        }
    }

    [Benchmark]
    public async Task Send_Logging_Validation()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_loggingValidationPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Void_Logging_Validation()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(_voidCommand, CancellationToken.None);
    }
}
