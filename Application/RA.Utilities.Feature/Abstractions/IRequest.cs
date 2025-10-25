namespace RA.Utilities.Feature.Abstractions;

/// <summary>
/// A marker interface for a feature (command or query).
/// This is used for features that do not return a value.
/// </summary>
public interface IRequest;

/// <summary>
/// A marker interface for a feature (command or query) that returns a value.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IRequest<TResponse> : IRequest;
