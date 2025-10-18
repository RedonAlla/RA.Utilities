using System;

namespace RA.Utilities.OpenApi.Settings;

/// <summary>
/// Open API Info Object, it provides the metadata about the Open API.
/// </summary>
public class OpenApiInfoSettings
{
    /// <summary>
    /// Value key in `appsettings.json` file.
    /// </summary>
    public const string AppSettingsKey = "OpenApiInfoSettings";

    /// <summary>
    /// Maps the Scalar API reference endpoint.
    /// </summary>
    public string? UiReferenceEndpoint { get; set; } = "/";

    /// <summary>
    /// The title of the application.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// A short summary of the API.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// A short description of the application.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The version of the OpenAPI document.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// A URL to the Terms of Service for the API.
    /// MUST be in the format of a URL.
    /// </summary>
    public Uri? TermsOfService { get; set; }

    /// <summary>
    /// The contact information for the exposed API.
    /// </summary>
    public OpenApiContactSettings? Contact { get; set; }

    /// <summary>
    /// The license information for the exposed API.
    /// </summary>
    public OpenApiLicenseSettings? License { get; set; }
}

/// <summary>
/// Contact Object.
/// </summary>
public class OpenApiContactSettings
{
    /// <summary>
    /// The identifying name of the contact person/organization.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The URL pointing to the contact information. MUST be in the format of a URL.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// The email address of the contact person/organization.
    /// MUST be in the format of an email address.
    /// </summary>
    public string? Email { get; set; }
}

/// <summary>
/// License Object.
/// </summary>
public class OpenApiLicenseSettings
{
    /// <summary>
    /// REQUIRED. The license name used for the API.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// An SPDX license expression for the API. The identifier field is mutually exclusive of the url field.
    /// </summary>
    public string? Identifier { get; set; }

    /// <summary>
    /// The URL pointing to the contact information. MUST be in the format of a URL.
    /// </summary>
    public Uri? Url { get; set; }
}
