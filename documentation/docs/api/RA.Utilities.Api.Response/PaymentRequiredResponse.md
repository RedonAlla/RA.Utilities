---
title: PaymentRequiredResponse
sidebar_position: 14
---

```bash
Namespace: RA.Utilities.Api.Response
```

The `PaymentRequiredResponse` class creates a standardized `402 Payment Required` response. It derives from [`Response<T>`](./Response) with `T = ErrorResult` and is used when an authenticated caller must pay before accessing the resource.

### 🎯 Purpose

Use `PaymentRequiredResponse` for paywalls, exhausted prepaid balances, and quotas that only a paid plan unlocks. It tells the client the request was understood and authenticated, but the account is not entitled to the resource yet.

- **Standard envelope**: `ResponseCode = 402`, `ResponseType = ResponseType.PaymentRequired`, `ResponseMessage = "Payment is required to access this resource."`
- **Optional payload**: `Result` can be `null` or an [`ErrorResult`](./ErrorResult) describing the payment condition.

### Constructors

| Constructor | `Result` | `ResponseCode` | `ResponseMessage` |
|---|---|---|---|
| `PaymentRequiredResponse(int responseCode, string responseMessage)` | `null` | `responseCode` | `responseMessage` |
| `PaymentRequiredResponse(ErrorResult? result = null)` | `result` | `402` | `"Payment is required to access this resource."` |
| `PaymentRequiredResponse(ErrorResult result, int? responseCode, string responseMessage = "")` | `result` | `responseCode ?? 402` | `responseMessage ?? standard` (the `""` default is used as-is) |

`ResponseType` is always `ResponseType.PaymentRequired` (JSON `18`). The optional `result` parameter means `new PaymentRequiredResponse()` is valid.

> `PaymentRequiredException` is translated by `RA.Utilities.Api` through the `(int responseCode, string responseMessage)` constructor, so the exception code and message appear on the envelope and `result` stays `null`.

### Usage

```csharp showLineNumbers
using Microsoft.AspNetCore.Mvc;
// highlight-next-line
using RA.Utilities.Api.Response;

[HttpGet("reports/{id}")]
public IActionResult GetReport(int id)
{
    if (!_plans.HasPaidPlan(User))
    {
        // highlight-next-line
        return StatusCode(402, new PaymentRequiredResponse());
    }

    return Ok(new SuccessResponse<Report>(_service.GetReport(id)));
}
```

To return a domain-specific reason, use the code/message constructor:

```csharp showLineNumbers
return StatusCode(402, new PaymentRequiredResponse(
    // highlight-next-line
    402,
    "This report is available on paid plans."));
```

### Example JSON Output

```json showLineNumbers
{
  "responseCode": 402,
  // highlight-next-line
  "responseType": 18,
  "responseMessage": "Payment is required to access this resource.",
  "result": null
}
```

With `JsonStringEnumConverter`, `responseType` is `"PaymentRequired"`. The member was added in `RA.Utilities.Core.Constants` v10.1.1, together with `BaseResponseCode.PaymentRequired` and `BaseResponseMessages.PaymentRequired`.
