namespace RA.Utilities.Feature.Benchmarks.Shared;

/// <summary>
/// The response payload produced by the ping handlers on both sides of the comparison.
/// </summary>
public sealed class PongResponse
{
    public PongResponse(string value) => Value = value;

    public string Value { get; }
}

/// <summary>
/// The typed data carried through the RA.Utilities.Feature pipeline context benchmark.
/// </summary>
public sealed class BenchmarkContext
{
    public int Counter { get; set; }
}

/// <summary>
/// Common payload contract implemented by every ping request on both sides of the comparison.
/// </summary>
internal interface IPingRequest
{
    string Value { get; }
}
