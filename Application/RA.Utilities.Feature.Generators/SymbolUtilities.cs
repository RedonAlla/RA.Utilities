using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace RA.Utilities.Feature.Generators;

/// <summary>
/// Shared symbol helpers used by the generators to check accessibility and to build the
/// fully qualified names emitted into generated code.
/// </summary>
internal static class SymbolUtilities
{
    /// <summary>
    /// Determines whether the generated code (which lives in a plain static class in the same
    /// assembly) can reference the given type.
    /// </summary>
    /// <param name="typeSymbol">The type to check, including its containing types.</param>
    /// <returns><see langword="true"/> when the type is accessible to the generated code.</returns>
    public static bool IsAccessibleToGeneratedCode(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal)
                || current.IsFileLocal)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Builds the fully qualified name (including the <c>global::</c> prefix) of the given type,
    /// with containers emitted outermost first.
    /// </summary>
    /// <param name="typeSymbol">The type to name.</param>
    /// <returns>The fully qualified name.</returns>
    public static string GetFullyQualifiedName(INamedTypeSymbol typeSymbol)
    {
        ImmutableArray<string>.Builder names = ImmutableArray.CreateBuilder<string>();
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            names.Add(current.Name);
        }

        names.Reverse();

        string qualifiedName = string.Join(".", names);
        string? @namespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : typeSymbol.ContainingNamespace.ToDisplayString();

        return @namespace is null
            ? $"global::{qualifiedName}"
            : $"global::{@namespace}.{qualifiedName}";
    }

    /// <summary>
    /// Determines whether the given type is declared in source code of the current compilation
    /// (as opposed to being loaded from referenced assembly metadata).
    /// </summary>
    /// <param name="typeSymbol">The type to check.</param>
    /// <returns><see langword="true"/> when the type is declared in the current compilation.</returns>
    public static bool IsDeclaredInCurrentCompilation(INamedTypeSymbol typeSymbol) =>
        !typeSymbol.DeclaringSyntaxReferences.IsDefaultOrEmpty;

    /// <summary>
    /// Determines whether a type from a referenced assembly (and all of its containing types)
    /// is public, so that generated code in the consuming assembly can reference it.
    /// </summary>
    /// <param name="typeSymbol">The type to check, including its containing types.</param>
    /// <returns><see langword="true"/> when the type is public and reachable from the generated code.</returns>
    public static bool IsPubliclyAccessible(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility != Accessibility.Public || current.IsFileLocal)
            {
                return false;
            }
        }

        return true;
    }
}
