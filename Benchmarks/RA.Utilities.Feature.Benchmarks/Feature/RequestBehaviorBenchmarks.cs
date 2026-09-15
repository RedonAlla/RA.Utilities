using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Core.Exceptions;
using RA.Utilities.Feature.Benchmarks.Shared;
using RA.Utilities.Feature.Generated;

namespace RA.Utilities.Feature.Benchmarks.Feature;

/// <summary>
/// Benchmarks the package's real request pipeline behaviors — the response and void
/// <c>LoggingBehavior</c> variants and the validation behaviors wired by <c>AddValidator</c> —
/// against a behavior-less baseline. Each scenario owns its request type (the pipeline depth
/// and behavior set are fixed by the DI registrations), and every iteration resolves the
/// generated mediator from a fresh scope to mirror per-request semantics. The failure scenario
/// measures the validation failure path (throw + catch inside the benchmark, so the run itself
/// stays green). See <c>MediatrBehaviorBenchmarks</c> for the MediatR mirror of these scenarios.
/// </summary>
[MemoryDiagnoser]
public class RequestBehaviorBenchmarks
{
    private readonly PingZeroRequest _barePing = new("payload");
    private readonly LoggingPingRequest _loggingPing = new("payload");
    private readonly ValidatedPingRequest _validationPingValid = new("payload");
    private readonly ValidatedPingRequest _validationPingInvalid = new(string.Empty);
    private readonly LoggingValidatedPingRequest _loggingValidationPing = new("payload");
    private readonly VoidBehaviorCommand _voidCommand = new("payload");
    private IServiceScopeFactory _scopeFactory = null!;

    [GlobalSetup]
    public void Setup()
    {
        _scopeFactory = DiHost.BuildScopeFactory(BehaviorServices.ConfigureRequestBehaviors);
    }

    [Benchmark]
    public async Task Send_NoBehaviors()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_barePing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Logging()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_loggingPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Validation_Passing()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_validationPingValid, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Validation_Failing()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        try
        {
            await mediator.Send(_validationPingInvalid, CancellationToken.None);
        }
        catch (BadRequestException ex)
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
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_loggingValidationPing, CancellationToken.None);
    }

    [Benchmark]
    public async Task Send_Void_Logging_Validation()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        Mediator mediator = scope.ServiceProvider.GetRequiredService<Mediator>();
        await mediator.Send(_voidCommand, CancellationToken.None);
    }
}
