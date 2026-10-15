using System.Diagnostics;

namespace RA.Utilities.Logging.Core.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Activity"/> class.
/// </summary>
public static class ActivityExtensions
{
    extension(Activity activity)
    {
        /// <summary>
        /// Gets a string representation of the activity identifier based on its format.
        /// </summary>
        /// <returns>
        /// The activity ID as a string. This will be the <see cref="Activity.Id"/> for <see cref="ActivityIdFormat.Hierarchical"/> format,
        /// or the <see cref="Activity.SpanId"/> for <see cref="ActivityIdFormat.W3C"/> format.
        /// Returns <c>null</c> if the activity is <c>null</c> or the format is <see cref="ActivityIdFormat.Unknown"/>.
        /// </returns>
        public string? GetActivityId()
        {
            return activity?.IdFormat switch
            {
                ActivityIdFormat.Hierarchical => activity.Id,
                ActivityIdFormat.W3C => activity.SpanId.ToHexString(),
                _ => null,
            };
        }
    }
}
