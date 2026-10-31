using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace RA.Utilities.Dispatcher.Options;

/// <summary>
/// Options that control which auto-discovered handlers and related behaviors
/// <c>AddDispatcher</c> registers, and which additional pipeline behaviors and validators it
/// registers explicitly.
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

    /// <summary>
    /// Gets the pipeline behaviors that <c>AddDispatcher</c> registers explicitly, in chain order:
    /// the first type is the outermost behavior and the last one runs closest to the handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepts closed and open generic implementations of
    /// <c>IPipelineBehavior&lt;TRequest&gt;</c>, <c>IPipelineBehavior&lt;TRequest, TResponse&gt;</c>,
    /// and <c>INotificationBehavior&lt;TNotification&gt;</c>. An open generic implementation, such as
    /// <c>typeof(LoggingBehavior&lt;,&gt;)</c>, is registered once under the open behavior interface and
    /// applies to every matching message; its type parameters must be passed to the behavior interface
    /// in declaration order.
    /// </para>
    /// <para>
    /// Configured behaviors are registered <b>before</b> the source-generated registrations, so they
    /// wrap auto-registered behaviors and form the outer part of the chain. Explicit registration keeps
    /// this list order — <c>PipelineBehaviorOrderAttribute</c> is not read for these types, exactly as
    /// for <c>AddDecoration</c>. A closed behavior that is also auto-registered is registered once, at
    /// its configured position. Entries are registered with <c>TryAddEnumerable</c>, so calling
    /// <c>AddDispatcher</c> more than once does not duplicate them.
    /// </para>
    /// <para>
    /// These registrations are explicit, so <see cref="IncludeHandlers"/> and
    /// <see cref="ExcludeHandlers"/> do not remove them.
    /// </para>
    /// </remarks>
    public List<Type> Behaviors { get; } = [];

    /// <summary>
    /// Gets or sets the lifetime used for the behaviors in <see cref="Behaviors"/>.
    /// The default is <see cref="ServiceLifetime.Transient"/>.
    /// </summary>
    public ServiceLifetime BehaviorLifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    /// Gets the validators that <c>AddDispatcher</c> registers, mapped to the lifetime used for the
    /// validator and its validation behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each entry registers the validator as <c>IValidator&lt;TRequest&gt;</c> and the matching
    /// <c>ValidationBehavior&lt;TRequest&gt;</c> or
    /// <c>ValidationBehavior&lt;TRequest, TResponse&gt;</c>, so one entry replaces the explicit
    /// validator and validation-behavior registrations:
    /// </para>
    /// <code>
    /// services.AddDispatcher(options =>
    /// {
    ///     options.Validators.Add(typeof(TodoCreateValidator), ServiceLifetime.Transient);
    /// });
    /// </code>
    /// <para>
    /// The request type is inferred from the validator's <c>IValidator&lt;TRequest&gt;</c> interface
    /// and the response type, when present, from the request's
    /// <c>IRequest&lt;TResponse&gt;</c> interface. A validator that the request type does not expose
    /// through <c>IRequest</c> cannot be mapped to a pipeline and is rejected.
    /// </para>
    /// <para>
    /// Validator types must be non-abstract, closed classes. Validation behaviors are registered
    /// after the types in <see cref="Behaviors"/>, so they run inside the behaviors configured there;
    /// list the validation behavior in <see cref="Behaviors"/> to control its position explicitly.
    /// Registrations use <c>TryAddEnumerable</c>, so explicit registrations and repeated
    /// <c>AddDispatcher</c> calls do not duplicate them.
    /// </para>
    /// <para>
    /// These registrations are explicit, so <see cref="IncludeHandlers"/> and
    /// <see cref="ExcludeHandlers"/> do not remove them.
    /// </para>
    /// </remarks>
    public Dictionary<Type, ServiceLifetime> Validators { get; } = [];
}
