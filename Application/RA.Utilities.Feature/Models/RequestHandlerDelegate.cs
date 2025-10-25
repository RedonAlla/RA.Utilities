using System.Threading.Tasks;
using RA.Utilities.Core.Results;

namespace RA.Utilities.Feature.Models;

/// <summary>
/// Represents a delegate for handling a request without a response.
/// </summary>
public delegate Task<Result> RequestHandlerDelegate();

/// <summary>
/// Represents a delegate for handling a request with a response.
/// </summary>
public delegate Task<Result<TResponse>> RequestHandlerDelegate<TResponse>();
