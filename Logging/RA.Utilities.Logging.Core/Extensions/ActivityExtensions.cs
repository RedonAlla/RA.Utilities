using System;
using System.Diagnostics;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="System.Diagnostics.Activity"/> class.
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// Gets a string representation of the activity identifier based on its format.
    /// </summary>
    /// <param name="activity">The activity instance from which to get the ID. This can be <c>null</c>.</param>
    /// <returns>
    /// The activity ID as a string. This will be the <see cref="Activity.Id"/> for <see cref="ActivityIdFormat.Hierarchical"/> format,
    /// or the <see cref="Activity.SpanId"/> for <see cref="ActivityIdFormat.W3C"/> format.
    /// If the activity is <c>null</c> or the format is <see cref="ActivityIdFormat.Unknown"/>, a new <see cref="Guid"/> string is returned.
    /// </returns>
    /// <remarks>
    /// This method provides a consistent way to retrieve a traceable ID from an activity,
    /// falling back to a new GUID if a specific ID cannot be determined from the activity's context.
    /// </remarks>
    public static string GetActivityId(this Activity? activity)
    {
        return activity?.IdFormat switch
        {
            ActivityIdFormat.Hierarchical => activity.Id,
            ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
            _ => null,
        } ?? Guid.NewGuid().ToString();
    }
}
