```powershell
Namespace: RA.Utilities.OpenApi.Settings
```

Its primary purpose is to allow developers to define a list of common HTTP headers in their `appsettings.json` file.
The [`HeadersParameterTransformer`](../DocumentTransformers//HeadersParameterTransformer.md) then reads these settings and automatically adds the specified headers to every API operation in the generated OpenAPI (Swagger) document.

## ✨ This approach has several key benefits:

1. **Centralized Configuration**: Instead of annotating every endpoint with attributes for common headers like `x-request-id` or `trace-id`, you can define them once in a single configuration file.
2. **Consistency**: It ensures that all endpoints consistently document the same set of required request headers and expected response headers.
3. **Reduced Boilerplate**: It keeps your controller and endpoint code clean by removing repetitive OpenAPI-specific attributes.

* **`AppSettingsKey`**: Defines the default configuration section (`"OpenApiHeaders"`) in `appsettings.json`.
* **`RequestHeaders`**: A list of `HeaderDefinition` objects that will be added as parameters to every API request.
* **`ResponseHeaders`**: A list of `HeaderDefinition` objects that will be added to every possible response for all API operations.

The class comes with sensible defaults, pre-configuring `x-request-id` for requests and both `x-request-id` and `trace-id` for responses, which are common headers in distributed systems for tracing and correlation.

## Properties

| Property           | Type                     | Description                                                                 |
| ------------------ | ------------------------ | --------------------------------------------------------------------------- |
| **RequestHeaders** | [`List<HeaderDefinition>`](#headerdefinition) | A list of header definitions to add to all API requests.  |
| **ResponseHeaders**| [`List<HeaderDefinition>`](#headerdefinition) | A list of header definitions to add to all API responses. |

### `HeaderDefinition`

Represents a single header to be added to the OpenAPI specification.

| Property      | Type      | Default      | Description                                                             |
| ------------- | --------- | ------------ | ----------------------------------------------------------------------- |
| **Name**        | `string`  | `""`         | The name of the header (e.g., "x-request-id").                          |
| **Description** | `string`  | `""`         | A description of the header's purpose.                                  |
| **Required**    | `bool`    | `true`       | Specifies if the header is required.                                    |
| **Type**        | `JsonSchemaType`| `String`| The schema type for the header value (e.g., "String", "Integer"). |
| **Format**      | `string`  | `"uuid"`     | The format of the header value (e.g., "uuid", "date-time").             |
| **Value**       | `object`  | `null`       | An example value for the header.                                        |


## Example `appsettings.json`
```json showLineNumbers
// appsettings.json
{
  "OpenApiHeaders": {
    "RequestHeaders": [
      {
        "Name": "x-request-id",
        "Description": "A unique identifier for the API call, used for tracing.",
        "Required": true,
        "Type": "String",
        "Format": "uuid"
      }
    ],
    "ResponseHeaders": [
      {
        "Name": "x-request-id",
        "Description": "The unique identifier of the request, echoed back for correlation."
      }
    ]
  }
}
```