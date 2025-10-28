using System;
using System.Diagnostics.CodeAnalysis;

namespace RA.Utilities.Core.Results;

/// <summary>
/// Represents the result of an operation, which can be either a success or a failure with an exception.
/// </summary>
/// <remarks>
/// This class is used to avoid throwing exceptions for expected error conditions.
/// It allows for a more functional approach to error handling.
/// </remarks>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class representing a successful operation.
    /// This constructor is protected to enforce the use of the static <see cref="Success()"/> factory method.
    /// </summary>
    protected Result()
    {
        // This constructor is for the success case.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class representing a failed operation.
    /// This constructor is protected to enforce the use of the static factory methods.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    protected Result(Exception exception)
    {
        Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether the result represents a failure.
    /// </summary>
    /// <value><see langword="true"/> if the result is a failure; otherwise, <see langword="false"/>.</value>
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool IsFailure => Exception != null;

    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    /// <value><see langword="true"/> if the result is a success; otherwise, <see langword="false"/>.</value>
    [MemberNotNullWhen(false, nameof(Exception))]
    public virtual bool IsSuccess => !IsFailure;

    /// <summary>
    /// Gets the exception of the result if the operation failed.
    /// Returns <see langword="null"/> if the operation was successful.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Creates a success result for an operation that does not return a value.
    /// </summary>
    /// <returns>A success <see cref="Result"/>.</returns>
    public static Result Success() => new();

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>A success <see cref="Result{TResult}"/>.</returns>
    public static Result<TResult> Success<TResult>(TResult value) => new(value);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failure <see cref="Result"/>.</returns>
    public static Result Failure(Exception exception) => new(exception);

    /// <summary>
    /// Creates a failure result for a specific result type.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failure <see cref="Result{TResult}"/>.</returns>
    public static Result<TResult> Failure<TResult>(Exception exception) => new(exception);

    /// <summary>
    /// Implicitly converts an exception to a failure <see cref="Result"/>.
    /// </summary>
    /// <param name="exception">The exception to wrap in a <see cref="Result"/>.</param>
    /// <returns>A new <see cref="Result"/> instance representing a failure.</returns>
    public static implicit operator Result(Exception exception) => new(exception);
}

/// <summary>
/// Represents the result of an operation, which can be either a success with a value or a failure with an exception.
/// </summary>
/// <typeparam name="TResult">The type of the value in case of success.</typeparam>
/// <remarks>
/// This class is used to avoid throwing exceptions for expected error conditions.
/// It allows for a more functional approach to error handling.
/// </remarks>
public sealed class Result<TResult> : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult}"/> class representing a successful operation.
    /// </summary>
    /// <param name="value">The result value.</param>
    public Result(TResult value)
    {
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TResult}"/> class representing a failed operation.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    public Result(Exception exception) : base(exception)
    {
    }

    /// <summary>
    /// Gets the value of the result if the operation was successful.
    /// Returns <see langword="default"/> if the operation failed.
    /// </summary>
    public TResult? Value { get; init; }

    /// <summary>
    /// Gets a value indicating whether the result represents a success.
    /// </summary>
    /// <value><see langword="true"/> if the result is a success; otherwise, <see langword="false"/>.</value>
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => !IsFailure;

    /// <summary>
    /// Implicitly converts a value to a success <see cref="Result{TResult}"/>.
    /// </summary>
    /// <param name="value">The value to wrap in a <see cref="Result{TResult}"/>.</param>
    /// <returns>A new <see cref="Result{TResult}"/> instance representing a success.</returns>
    public static implicit operator Result<TResult>(TResult value) => new(value);

    /// <summary>
    /// Implicitly converts an exception to a failure <see cref="Result{TResult}"/>.
    /// </summary>
    /// <param name="exception">The exception to wrap in a <see cref="Result{TResult}"/>.</param>
    /// <returns>A new <see cref="Result{TResult}"/> instance representing a failure.</returns>
    public static implicit operator Result<TResult>(Exception exception) => new(exception);

    /// <summary>
    /// Executes one of the provided functions based on whether the result is a success or a failure.
    /// </summary>
    /// <typeparam name="TContract">The return type of the match functions.</typeparam>
    /// <param name="success">The function to execute if the result is a success. The function takes the result value as a parameter.</param>
    /// <param name="failure">The function to execute if the result is a failure. The function takes the exception as a parameter.</param>
    /// <returns>The value returned by the executed function.</returns>
    public TContract Match<TContract>(Func<TResult, TContract> success, Func<Exception, TContract> failure) =>
        IsSuccess ? success(Value!) : failure(Exception!);
}
