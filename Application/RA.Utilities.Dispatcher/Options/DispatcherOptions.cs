using System;
using System.Collections.Generic;

namespace RA.Utilities.Dispatcher.Options;

/// <summary>
/// Options that control which auto-discovered handlers and related behaviors
/// <c>AddDispatcher</c> registers.
/// </summary>
public sealed class DispatcherOptions
{
    /// <summary>
    /// Gets or sets the handler types to include in auto-registration.
    /// When <see langword="null"/>, this list is ignored.
    /// When set, only the listed handlers — and the pipeline/notification behaviors related to
    /// the messages they handle — are registered.
    /// </summary>
    /// <remarks>
    /// Behavior types may be listed as well; a listed behavior is kept even when the handler of
    /// its message is filtered out.
    /// </remarks>
    public List<Type>? IncludeHandlers { get; set; }

    /// <summary>
    /// Gets or sets the handler types to exclude from auto-registration.
    /// When <see langword="null"/>, this list is ignored.
    /// When set, the listed handlers are not registered, and neither are the pipeline/notification
    /// behaviors related to a message once no other registered handler handles it.
    /// </summary>
    /// <remarks>
    /// Behavior types may be listed as well, which removes them even when their handler is kept.
    /// Exclusions win over inclusions.
    /// </remarks>
    public List<Type>? ExcludeHandlers { get; set; }
}
