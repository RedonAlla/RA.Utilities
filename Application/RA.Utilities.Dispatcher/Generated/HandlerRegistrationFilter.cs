using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using RA.Utilities.Dispatcher.Abstractions;
using RA.Utilities.Dispatcher.Options;

namespace RA.Utilities.Dispatcher.Generated;

/// <summary>
/// Applies <see cref="DispatcherOptions"/> to the descriptors added by the handler
/// auto-registration callbacks. Handlers are selected by their implementation type; pipeline and
/// notification behaviors follow the handlers of the message they are closed over.
/// </summary>
internal static class HandlerRegistrationFilter
{
    /// <summary>
    /// Removes the auto-registered handler and behavior descriptors that the options filter out.
    /// </summary>
    /// <param name="services">The service collection that received the auto-registrations.</param>
    /// <param name="startIndex">The index of the first descriptor added by the auto-registrations.</param>
    /// <param name="options">The include/exclude options.</param>
    public static void Apply(IServiceCollection services, int startIndex, DispatcherOptions options)
    {
        HashSet<Type>? include = options.IncludeHandlers is null ? null : new HashSet<Type>(options.IncludeHandlers);
        HashSet<Type>? exclude = options.ExcludeHandlers is null ? null : new HashSet<Type>(options.ExcludeHandlers);

        if (include is null && exclude is null)
        {
            return;
        }

        Dictionary<Type, HashSet<Type>> handlerMessages = CollectHandlerMessages(services, startIndex);
        HashSet<Type> selectedMessages = SelectHandlers(handlerMessages, include, exclude);

        for (int i = services.Count - 1; i >= startIndex; i--)
        {
            if (!ShouldKeep(services[i], handlerMessages, selectedMessages, include, exclude))
            {
                services.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Maps every auto-registered handler implementation to the messages it handles, so that
    /// behaviors can be associated with the handlers of their message.
    /// </summary>
    private static Dictionary<Type, HashSet<Type>> CollectHandlerMessages(IServiceCollection services, int startIndex)
    {
        var handlerMessages = new Dictionary<Type, HashSet<Type>>();

        for (int i = startIndex; i < services.Count; i++)
        {
            ServiceDescriptor descriptor = services[i];
            if (descriptor.ImplementationType is not Type handler
                || !TryGetHandlerMessage(descriptor.ServiceType, out Type? message))
            {
                continue;
            }

            if (!handlerMessages.TryGetValue(handler, out HashSet<Type>? messages))
            {
                messages = [];
                handlerMessages.Add(handler, messages);
            }

            messages.Add(message);
        }

        return handlerMessages;
    }

    /// <summary>
    /// Returns the messages that remain represented once the include/exclude lists are applied.
    /// </summary>
    private static HashSet<Type> SelectHandlers(
        Dictionary<Type, HashSet<Type>> handlerMessages,
        HashSet<Type>? include,
        HashSet<Type>? exclude)
    {
        var selectedMessages = new HashSet<Type>();

        foreach ((Type handler, HashSet<Type> messages) in handlerMessages)
        {
            if (IsHandlerSelected(handler, include, exclude))
            {
                selectedMessages.UnionWith(messages);
            }
        }

        return selectedMessages;
    }

    private static bool ShouldKeep(
        ServiceDescriptor descriptor,
        Dictionary<Type, HashSet<Type>> handlerMessages,
        HashSet<Type> selectedMessages,
        HashSet<Type>? include,
        HashSet<Type>? exclude)
    {
        if (descriptor.ImplementationType is not Type implementation)
        {
            return true;
        }

        // Handlers are registered under their interface and as themselves; both descriptors are
        // selected by the same implementation type.
        if (handlerMessages.ContainsKey(implementation))
        {
            return IsHandlerSelected(implementation, include, exclude);
        }

        if (!TryGetBehaviorMessage(descriptor.ServiceType, out Type? message))
        {
            return true;
        }

        if (exclude is not null && exclude.Contains(implementation))
        {
            return false;
        }

        // A behavior listed for inclusion is kept even if the handler of its message is filtered out.
        if (include is not null && include.Contains(implementation))
        {
            return true;
        }

        // Otherwise follow the message: keep the behavior while at least one handler for that
        // message is registered. A behavior whose message has no handler at all is left alone
        // unless an include list restricts registration.
        return selectedMessages.Contains(message)
            || include is null && !HasHandlers(handlerMessages, message);
    }

    private static bool IsHandlerSelected(Type handler, HashSet<Type>? include, HashSet<Type>? exclude) =>
        (include is null || include.Contains(handler)) && (exclude is null || !exclude.Contains(handler));

    private static bool HasHandlers(Dictionary<Type, HashSet<Type>> handlerMessages, Type message)
    {
        foreach (HashSet<Type> messages in handlerMessages.Values)
        {
            if (messages.Contains(message))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetHandlerMessage(Type serviceType, [NotNullWhen(true)] out Type? message)
    {
        if (serviceType.IsGenericType)
        {
            Type definition = serviceType.GetGenericTypeDefinition();
            if (definition == typeof(IRequestHandler<>)
                || definition == typeof(IRequestHandler<,>)
                || definition == typeof(INotificationHandler<>))
            {
                message = serviceType.GetGenericArguments()[0];
                return true;
            }
        }

        message = null;
        return false;
    }

    private static bool TryGetBehaviorMessage(Type serviceType, [NotNullWhen(true)] out Type? message)
    {
        if (serviceType.IsGenericType)
        {
            Type definition = serviceType.GetGenericTypeDefinition();
            if (definition == typeof(IPipelineBehavior<>)
                || definition == typeof(IPipelineBehavior<,>)
                || definition == typeof(INotificationBehavior<>))
            {
                message = serviceType.GetGenericArguments()[0];
                return true;
            }
        }

        message = null;
        return false;
    }
}
