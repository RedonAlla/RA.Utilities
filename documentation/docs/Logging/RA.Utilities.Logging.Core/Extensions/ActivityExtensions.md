```bash
Namespace: RA.Utilities.Logging.Core.Extensions
```

The `ActivityExtensions` class provides a trace-identifier helper for [`System.Diagnostics.Activity`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.activity).

Its single extension member, `GetActivityId()`, returns a string identifier for the current activity, normalising the two formats the framework can use. It is the fallback source of the `TraceId` property inside [`RequestIdEnricher`](../Enrichers/index.md) when no `HttpContext` is available — for example in background jobs, console applications, or startup code.

:::info v10.1.1
The class is now written as a C# 14 extension member instead of a classic extension method. The public method name, return type (`string?`), and namespace are unchanged — see the [migration guide](../migration-guides.mdx).
:::

## Methods

### `GetActivityId()`

Gets a string representation of the activity identifier, based on the activity's `IdFormat`.

#### Parameters

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| **activity** | `Activity` | The activity to read the identifier from. This is the **receiver** of the extension member and is declared non-nullable. |

#### Returns

| Type | Description |
| ---- | ----------- |
| `string?` | The activity id, or `null` when there is no trace context. |

#### Mapping

| `IdFormat` | Returned value |
| ---------- | -------------- |
| `ActivityIdFormat.Hierarchical` | `Activity.Id` (the W3C-style predecessor format) |
| `ActivityIdFormat.W3C` | `Activity.SpanId.ToHexString()` |
| `ActivityIdFormat.Unknown` | `null` |
| Receiver is `null` | `null` |

#### Example

Call the member with `?.` so a missing ambient activity stays a no-op — this is the same pattern `RequestIdEnricher` uses internally:

```csharp showLineNumbers
using System.Diagnostics;
using RA.Utilities.Logging.Core.Extensions;
using Serilog;

using var activity = new Activity("background-job").Start();

// highlight-next-line
string? traceId = Activity.Current?.GetActivityId();

Log.Information("Background job started with trace {TraceId}", traceId);
```

Because the return value is nullable, store it in a `string?` and decide how to handle the "no trace context" case:

```csharp
// ✅ Works — null when there is no activity
string? traceId = Activity.Current?.GetActivityId();

// ❌ Avoid — null-forgiving hides a genuinely absent trace context
string traceId = Activity.Current?.GetActivityId()!;
```

For automatic trace-context propagation across an entire application, prefer `ActivitySource` with OpenTelemetry over creating activities by hand.
