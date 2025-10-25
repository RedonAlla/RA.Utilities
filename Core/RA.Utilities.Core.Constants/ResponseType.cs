using System.Text.Json.Serialization;

namespace RA.Utilities.Core.Constants;

/// <summary>
/// Defines the various types of responses that can be returned from a service operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ResponseType>))]
public enum ResponseType
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The request failed due to validation errors.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// An unexpected problem occurred.
    /// This is often used for RFC 7807 problem details.
    /// </summary>
    Problem = 2,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// The request could not be completed due to a conflict with the current state of the resource.
    /// </summary>
    Conflict = 4,

    /// <summary>
    /// An error occurred while interacting with the database.
    /// </summary>
    Database = 5,

    /// <summary>
    /// The request requires user authentication.
    /// </summary>
    Unauthorized = 6,

    /// <summary>
    /// An unknown or unexpected error occurred.
    /// </summary>
    Unknown = 7,

    /// <summary>
    /// A general error occurred during the operation.
    /// </summary>
    Error = 8,

    /// <summary>
    /// The server cannot or will not process the request due to something that is perceived to be a client error.
    /// </summary>
    BadRequest = 9
}
