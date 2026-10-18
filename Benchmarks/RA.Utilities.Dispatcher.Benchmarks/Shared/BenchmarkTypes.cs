namespace RA.Utilities.Dispatcher.Benchmarks.Shared;

/// <summary>
/// The response payload produced by every response handler on all three sides of the comparison.
/// </summary>
public sealed class PongResponse
{
    public PongResponse(string value) => Value = value;

    public string Value { get; }
}

/// <summary>
/// Common payload contract implemented by every ping request on all three sides of the comparison.
/// </summary>
internal interface IPingRequest
{
    string Value { get; }
}
