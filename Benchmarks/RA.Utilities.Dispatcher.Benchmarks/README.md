# RA.Utilities.Dispatcher Benchmarks

BenchmarkDotNet suite comparing the new **`RA.Utilities.Dispatcher`** package against the
original **`RA.Utilities.Feature`** package and **MediatR** on identical scenarios.

Both RA packages ship their source generator into this assembly, so every benchmark exercises a
real generated `Mediator` and real DI registrations — nothing is stubbed.

## Scenarios

| Scenario | Dispatcher | Feature | MediatR |
|---|---|---|---|
| Send, no behaviors | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` |
| Send, one behavior | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` |
| Send, three behaviors | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` |
| Void send | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` (Unit) |
| Void send, three behaviors | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` (Unit) |
| Publish, one handler | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` |
| Publish, three handlers | `IDispatcher` + concrete `Mediator` | `IMediator` + concrete `Mediator` | `IMediator` |

Every iteration creates a DI scope and resolves the mediator from it, mirroring per-request
semantics. Handlers are no-ops and loggers are null loggers, so the measurements isolate the
dispatch machinery. MediatR is the baseline (`Baseline = true`) in each table.

> **Note:** Dispatcher handlers return `ValueTask<Result<T>>` / `ValueTask<Result>` and
> `IDispatcher` dispatch returns the same, while the Feature and MediatR sides still return
> `Task<T>` / `Task`. The Dispatcher numbers therefore include the `Result` wrapping and
> `ValueTask` overhead; the comparison is not return-type-identical.

## Run

```bash
# Everything (long)
dotnet run -c Release --project Benchmarks/RA.Utilities.Dispatcher.Benchmarks

# One table
dotnet run -c Release --project Benchmarks/RA.Utilities.Dispatcher.Benchmarks \
  -- --filter "*SendComparisonBenchmarks*"

# Quick sanity run
dotnet run -c Release --project Benchmarks/RA.Utilities.Dispatcher.Benchmarks \
  -- --filter "*SendComparisonBenchmarks*" --job dry
```

## Expected generator diagnostics

The request handlers and no-op pipeline behaviors are generic so they can be registered
explicitly and identically on all three sides. Both generators therefore report their
"generic handler / generic behavior / no discoverable handler" diagnostics
(`DSAG002`, `DSAG007`, `DSAG010`, `FEAG002`, `FEAG007`, `FEAG010`); they are suppressed in the
project file and do not affect the measurements.
