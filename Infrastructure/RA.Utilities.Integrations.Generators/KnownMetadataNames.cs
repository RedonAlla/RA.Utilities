namespace RA.Utilities.Integrations.Generators;

/// <summary>
/// Contains the fully qualified metadata names used by the generator to match marker attributes
/// and to reference the runtime types living in the <c>RA.Utilities.Integrations</c> package.
/// </summary>
internal static class KnownMetadataNames
{
    /// <summary>
    /// The metadata name of the <c>[QueryParameters]</c> marker attribute.
    /// </summary>
    public const string QueryParametersAttribute = "RA.Utilities.Integrations.Attributes.QueryParametersAttribute";

    /// <summary>
    /// The metadata name of the <c>[HeaderParameters]</c> marker attribute.
    /// </summary>
    public const string HeaderParametersAttribute = "RA.Utilities.Integrations.Attributes.HeaderParametersAttribute";

    /// <summary>
    /// The fully qualified name of the <c>[QueryParameterName]</c> property attribute.
    /// </summary>
    public const string QueryParameterNameAttribute = "global::RA.Utilities.Integrations.Attributes.QueryParameterNameAttribute";

    /// <summary>
    /// The fully qualified name of the <c>[HeaderParameterName]</c> property attribute.
    /// </summary>
    public const string HeaderParameterNameAttribute = "global::RA.Utilities.Integrations.Attributes.HeaderParameterNameAttribute";

    /// <summary>
    /// The fully qualified name of the query string request contract implemented by generated code.
    /// </summary>
    public const string IQueryStringRequest = "global::RA.Utilities.Integrations.Abstractions.IQueryStringRequest";

    /// <summary>
    /// The fully qualified name of the header request contract implemented by generated code.
    /// </summary>
    public const string IHeaderRequest = "global::RA.Utilities.Integrations.Abstractions.IHeaderRequest";

    /// <summary>
    /// The fully qualified name of the query parameter collection type.
    /// </summary>
    public const string QueryParams = "global::RA.Utilities.Integrations.Models.QueryParams";

    /// <summary>
    /// The name of the query string values member.
    /// </summary>
    public const string QueryStringValuesMethod = "QueryStringValues";

    /// <summary>
    /// The name of the headers member.
    /// </summary>
    public const string ToHeadersMethod = "ToHeaders";

    /// <summary>
    /// The name of the query string convenience member.
    /// </summary>
    public const string ToQueryStringMethod = "ToQueryString";

    /// <summary>
    /// The fully qualified name of the non-generic dictionary interface.
    /// </summary>
    public const string SystemCollectionsIDictionary = "global::System.Collections.IDictionary";

    /// <summary>
    /// The fully qualified name of the generic read-only dictionary interface.
    /// </summary>
    public const string GenericIReadOnlyDictionary = "global::System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>";

    /// <summary>
    /// The fully qualified name of the non-generic enumerable interface.
    /// </summary>
    public const string SystemCollectionsIEnumerable = "global::System.Collections.IEnumerable";

    /// <summary>
    /// The fully qualified name of the generic enumerable interface.
    /// </summary>
    public const string GenericIEnumerableOfT = "global::System.Collections.Generic.IEnumerable<T>";
}
