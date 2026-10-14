```powershell
Namespace: RA.Utilities.OpenApi.Settings
```

Its primary role is to act as a strongly-typed model that can be bound to a section in your `appsettings.json` file.
This allows you to define your API's high-level information—like its **title***, ***version***, ***description***, and contact details—in a configuration file instead of hard-coding it in your application's source code.

This class works directly with the [`DocumentInfoTransformer`](../DocumentTransformers/DocumentInfoTransformer.md), which reads the populated settings and applies them to the generated OpenAPI document.

Here's a breakdown of its key benefits:

1. **Decouples Configuration from Code**: You can update your API's title, version, or contact email without recompiling and redeploying your application.
2. **Environment-Specific Documentation**: It enables you to have different descriptions or contact points for your ***Development***, ***Staging***, and
***Production*** environments by using environment-specific `appsettings.json` files.
3. **Clean Startup Code**: It keeps your `Program.cs` file cleaner by moving static configuration data to the appropriate configuration files.


### Properties

| Property           | Type                               | Description                                                      |
| ------------------ | ---------------------------------- | ---------------------------------------------------------------- |
| **UiReferenceEndpoint** | `string`                      | The endpoint where the Scalar API reference UI is mapped. Defaults to `"/"`. |
| **Title**          | `string`                           | The title of the API.                                            |
| **Summary**        | `string`                           | A short summary of the API. Populated into the document since v10.1.1. |
| **Version**        | `string`                           | The version of the API.                                          |
| **Description**    | `string`                           | A short description of the API.                                  |
| **TermsOfService** | `string` (URI)                     | A URL to the Terms of Service for the API.                       |
| **Contact**        | [`OpenApiContactSettings`](#openapicontactsettings) (object)  | The contact information for the exposed API. See details below.  |
| **License**        | [`OpenApiLicenseSettings`](#openapilicensesettings) (object)  | The license information for the exposed API. See details below.  |

#### `OpenApiContactSettings`

| Property  | Type           | Description                               |
| --------- | -------------- | ----------------------------------------- |
| **Name**  | `string`       | The identifying name of the contact person/organization. |
| **Email** | `string`       | The email address of the contact person/organization. |
| **Url**   | `string` (URI) | The URL pointing to the contact information. |

#### `OpenApiLicenseSettings`

| Property | Type           | Description                            |
| -------- | -------------- | -------------------------------------- |
| Name     | `string`       | The license name used for the API.     |
| Identifier | `string`     | An SPDX license expression for the API. The identifier field is mutually exclusive of the url field. |
| Url      | `string` (URI) | A URL to the license used for the API. |

### Example `appsettings.json`

```json showLineNumbers
{
  "OpenApiInfoSettings": {
    "UiReferenceEndpoint": "/",
    "Title": "My Awesome API",
    "Version": "v1.0.0",
    "Summary": "A brief and catchy summary of the API.",
    "Description": "This is a more detailed description of the API. It can include **Markdown** for rich text formatting, explaining what the API does, its main features, and how to get started.",
    "TermsOfService": "https://example.com/terms",
    "Contact": {
      "Name": "API Support Team",
      "Url": "https://example.com/support",
      "Email": "support@example.com"
    },
    "License": {
      "Name": "MIT License",
      "Url": "https://opensource.org/licenses/MIT"
    }
  }
}
```