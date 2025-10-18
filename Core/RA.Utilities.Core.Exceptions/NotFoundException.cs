using System;
using System.Runtime.Serialization;
using RA.Utilities.Core.Constants;
using RA.Utilities.Core.Exceptions;

namespace RA.Utilities.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a requested resource could not be found (HTTP 404).
/// </summary>
[Serializable]
public class NotFoundException : RaBaseException
{
    /// <summary>
    /// Gets the name of the entity that was not found (e.g., "User").
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the value or identifier of the entity that was not found.
    /// </summary>
    public object EntityValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a resource that was not found.
    /// </summary>
    /// <param name="entityName">The name of the entity type that was not found.</param>
    /// <param name="entityValue">The value or identifier used to search for the entity.</param>
    /// <param name="code">The HTTP status code. Defaults to 404 (Not Found).</param>
    public NotFoundException(string entityName, object entityValue, int code = BaseResponseCode.NotFound)
        : base(code, $"{entityName} with value '{entityValue}' was not found.")
    {
        EntityName = entityName;
        EntityValue = entityValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        EntityName = info.GetString(nameof(EntityName))!;
        EntityValue = info.GetValue(nameof(EntityValue), typeof(object))!;
    }
}
