---
title: ServiceUnavailableResponse
sidebar_position: 10
---

```powershell
Namespace: RA.Utilities.Api.Results
```

The `ServiceUnavailableResponse` class is a specialized model for creating standardized `503 Service Unavailable` responses.
It is used when the server is temporarily unable to handle the request.
It inherits from [`Response<T>`](./Response), with the `Result` property typed as an [`ErrorResult`](./ErrorResult) object.

### 🎯 Purpose

The `ServiceUnavailableResponse` class is a specialized model for creating standardized `503 Service Unavailable` API responses.
It is used to signal that the server is temporarily unable to handle the request, often due to maintenance or overload.

Its primary functions are:

1. **Standardizes Availability Errors**: It provides a consistent structure for all `503 Service Unavailable` errors.

2. **Reduces Boilerplate**: It automatically sets the response properties for an unavailable service:

  * **ResponseCode**: Set to `503` (from `BaseResponseCode.ServiceUnavailable`).
  * **ResponseType**: Set to `ResponseType.ServiceUnavailable`.
  * **ResponseMessage**: Defaults to `"The service is temporarily unavailable. Please try again later."` (from `BaseResponseMessages.ServiceUnavailable`).

3. **Provides Structured Context**: It uses an [`ErrorResult`](./ErrorResult) payload to provide specific details about why the service is unavailable.

### ⚙️ How It Works

When you create an instance of `ServiceUnavailableResponse`, it pre-configures the following properties:

- **`ResponseCode`**: Set to `503` (from `BaseResponseCode.ServiceUnavailable`).
- **`ResponseType`**: Set to `ResponseType.ServiceUnavailable`.
- **`ResponseMessage`**: Defaults to `"The service is temporarily unavailable. Please try again later."`.
- **`Result`**: An [`ErrorResult`](./ErrorResult) object containing the error code and message.

### 🚀 Usage in a Controller

You can use this class in your controller actions or middleware when the service is temporarily unavailable.

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Results;

[ApiController]
[Route("api/[controller]")]
public sealed class WeatherController : ControllerBase
{
    [HttpGet("forecast")]
    public IActionResult GetForecast()
    {
        if (IsUnderMaintenance())
        {
            // highlight-next-line
            return StatusCode(503, new ServiceUnavailableResponse(new ErrorResult 
            { 
                ErrorCode = "MaintenanceMode", 
                ErrorMessage = "The service is currently under maintenance. Please try again later." 
            }));
        }

        return Ok(new SuccessResponse<string>("Forecast Data"));
    }
}
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 503,
  "responseType": "ServiceUnavailable",
  "responseMessage": "The service is temporarily unavailable. Please try again later.",
  "result": {
    "errorCode": "MaintenanceMode",
    "errorMessage": "The service is currently under maintenance. Please try again later."
  }
}
```
