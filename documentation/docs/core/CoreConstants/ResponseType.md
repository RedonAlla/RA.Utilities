```bash
Namespace: RA.Utilities.Core.Constants
```

The `ResponseType` enum is a core component of the `RA.Utilities` ecosystem, designed to provide a standardized, machine-readable vocabulary for the outcomes of API operations.
It is located in the `RA.Utilities.Core.Constants` package.

While HTTP status codes (like `200`, `404`, `500`) tell a client what happened at the transport level, the `ResponseType` enum provides more specific, semantic context about the business-level outcome within the JSON response body.

For example, an HTTP `400 Bad Request` is a generic client error. The `ResponseType` can clarify the cause:

* **ResponseType.Validation**: The request failed because one or more input fields were invalid.
* **ResponseType.BadRequest**: The request was malformed or syntactically incorrect in a way that goes beyond simple validation.

The primary purpose is to create a consistent contract for your APIs. A client application can parse the `responseType` field in the JSON payload to trigger specific logic (e.g., display validation errors on the correct form fields) without having to guess the meaning of a generic HTTP status code.

The `[JsonConverter(typeof(JsonStringEnumConverter<ResponseType>))]` attribute is crucial.
It ensures that when the enum is serialized to JSON, it uses its string name (e.g., `"NotFound"`) instead of its integer value (e.g., `3`), making the API response self-documenting and easy for developers to read.

## Constant Members

| Enum Member      | Description                                                                 | Typical HTTP Status |
|------------------|-----------------------------------------------------------------------------|---------------------|
| **Success**      | The operation was successful.                                               | 200 OK              |
| **Validation**   | The request failed due to invalid input data.                               | 400 Bad Request     |
| **Problem**      | An unexpected problem occurred, often used for RFC 7807 problem details.    | 500 Internal Server |
| **NotFound**     | The requested resource was not found.                                       | 404 Not Found       |
| **Conflict**     | The request conflicts with the current state of the resource.               | 409 Conflict        |
| **Unauthorized** | The request requires user authentication.                                   | 401 Unauthorized    |
| **Error**        | A general, non-specific error occurred during the operation.                | 500 Internal Server |
| **BadRequest**   | The request was malformed or could not be processed for reasons other than validation. | 400 Bad Request     |

## Example JSON Response

```json showLineNumbers
{
  "responseCode": 404,
  // highlight-next-line
  "responseType": "NotFound", // From the ResponseType enum
  "responseMessage": "Product with value '99' not found."
}
```