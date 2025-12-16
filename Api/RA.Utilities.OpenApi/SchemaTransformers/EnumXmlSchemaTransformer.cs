using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RA.Utilities.OpenApi.SchemaTransformers;

/// <summary>
/// Transforms OpenAPI schema descriptions for enum types by appending XML documentation comments
/// for each enum value as a Markdown table.
/// </summary>
internal sealed class EnumXmlSchemaTransformer : IOpenApiSchemaTransformer
{
    private readonly string _xmlPath;
    private XDocument? _xmlDocCache;
    private readonly Lock _xmlDocLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumXmlSchemaTransformer"/> class.
    /// </summary>
    /// <param name="xmlPath">The path to the XML documentation file.</param>
    public EnumXmlSchemaTransformer(string xmlPath)
    {
        _xmlPath = xmlPath;
    }

    /// <summary>
    /// Transforms the OpenAPI schema for enum types by appending a Markdown table of value descriptions
    /// extracted from the XML documentation file.
    /// </summary>
    /// <param name="schema">The OpenAPI schema to transform.</param>
    /// <param name="context">The schema transformer context.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        Type type = context.JsonTypeInfo.Type;

        // Only target Enums
        if (!type.IsEnum)
        {
            return Task.CompletedTask;
        }

        //TODO Add support for multiple files.
        // Cache the loaded XML document for performance
        XDocument xmlDoc = GetOrLoadXmlDoc();

        if (xmlDoc is null)
        {
            return Task.CompletedTask;
        }

        var enumDescriptions = new StringBuilder();
        enumDescriptions.AppendLine("| Value | Description |");
        enumDescriptions.AppendLine("| ----- | ----------- |");

        IEnumerable<XElement> members = xmlDoc.Descendants("member");

        foreach (string name in Enum.GetNames(type))
        {
            string memberName = $"F:{type.FullName}.{name}";

            // Use a single pass to find the member element
            XElement? member = FindMemberElement(members, memberName);

            if (member is null)
            {
                continue;
            }

            string? summary = member.Element("summary")?.Value.Trim();
            string? remarks = member.Element("remarks")?.Value.Trim();

            if (!string.IsNullOrEmpty(summary))
            {
                if (!string.IsNullOrEmpty(remarks))
                {
                    summary += $" {remarks}";
                }
                enumDescriptions.AppendLine(CultureInfo.InvariantCulture, $"| {name} | {summary} |");
            }
        }

        if (enumDescriptions.Length > 2)
        {
            schema.Description = (schema.Description ?? "") +
                "\n\n" + enumDescriptions;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Lazily loads and caches the XML documentation from the file path.
    /// This method is thread-safe.
    /// </summary>
    /// <returns>The loaded <see cref="XDocument"/>, or <c>null</c> if the file does not exist.</returns>
    private XDocument GetOrLoadXmlDoc()
    {
        // Use local variable to avoid CA1508 false positive
        XDocument? xmlDoc = _xmlDocCache;
        if (xmlDoc is not null)
        {
            return xmlDoc;
        }

        lock (_xmlDocLock)
        {
            if (_xmlDocCache == null)
            {
                // Use File.Exists to check if the XML documentation file exists before opening
                if (!File.Exists(_xmlPath))
                {
                    return xmlDoc;
                    // Ignore errors
                    //throw new FileNotFoundException($"The XML documentation file was not found: {_xmlPath}");
                }

                using FileStream stream = File.OpenRead(_xmlPath);
                _xmlDocCache = XDocument.Load(stream);
            }

            return _xmlDocCache!;
        }
    }

    /// <summary>
    /// Finds the XML element for a specific member from the list of all members.
    /// </summary>
    /// <param name="members">The collection of member elements from the XML documentation.</param>
    /// <param name="memberName">The name of the member to find (e.g., "F:MyNamespace.MyEnum.MyValue").</param>
    /// <returns>The <see cref="XElement"/> for the member, or <c>null</c> if not found.</returns>
    private static XElement? FindMemberElement(IEnumerable<XElement> members, string memberName) =>
        members.FirstOrDefault(m => m.Attribute("name")?.Value == memberName);
}
