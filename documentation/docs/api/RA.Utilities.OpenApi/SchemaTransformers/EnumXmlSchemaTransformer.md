```powershell
Namespace: RA.Utilities.OpenApi.SchemaTransformers
```

The `EnumXmlSchemaTransformer` is a utility designed to solve a common problem in API documentation: making enum types understandable to API consumers.

### 🎯 Key Purpose

The primary purpose of the `EnumXmlSchemaTransformer` is to **automatically enrich your OpenAPI (Swagger) schema by reading the XML documentation comments from your C# enums and appending them as a descriptive Markdown table**.

By default, an enum in a Swagger UI or Scalar might only show its names (e.g., "Value1", "Value2") or integer values (0, 1), leaving developers to guess their meaning. This transformer fixes that by turning this:

```csharp
/// <summary>
/// Represents the status of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// The order has been placed but not yet processed.
    /// </summary>
    Pending,

    /// <summary>
    /// The order has been processed and is ready for shipment.
    /// </summary>
    Processed,

    /// <summary>
    /// The order has been shipped to the customer.
    /// </summary>
    Shipped
}
```

...into this in your OpenAPI documentation:

Represents the status of an order.

| Value | Description |
| ----- | ----------- |
| Pending | The order has been placed but not yet processed. |
| Processed | The order has been processed and is ready for shipment. |
| Shipped | The order has been shipped to the customer. |

### ⚙️ How It Works
The process is straightforward:

1. It identifies any type in your API that is an `enum`.
2. It loads the XML documentation file that your project generates during the build process.
For performance, it caches this file in memory after the first read.
3. For each member of the enum, it finds the corresponding `<member>` tag in the XML file.
4. It extracts the text from the `<summary>` tag for that member.
5. It formats all the enum values and their summaries into a clean Markdown table.
6. Finally, it appends this table to the enum's main description in the OpenAPI schema.

### ✨ Key Benefits
* **Self-Documenting Code**: Your XML comments, which live right next to your code, become the single source of truth for your API documentation.
* **Reduces Ambiguity**: API consumers no longer have to guess what enum values like `0`, `1`, or `2` mean.
The documentation provides clear, human-readable text.
* **Improves Developer Experience**: It saves frontend developers and API consumers time by providing all the necessary context directly in the Swagger/OpenAPI UI.

### 🚀 How to use it

#### Step 1: Enable XML Documentation

First, you need to configure your project to generate an XML documentation file. You can do this by adding the `GenerateDocumentationFile` property to your `.csproj` file:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn> <!-- Optional: Suppresses warnings about missing XML comments -->
</PropertyGroup>
```

#### Step 2: Register the Transformer

Next, call the `AddEnumXmlDescriptionTransformer()` extension method when configuring your OpenAPI services in `Program.cs`. You'll need to provide the path to the generated XML file.

```csharp
// In Program.cs
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddOpenApi(options =>
{
    // ... other configurations
    // highlight-next-line
    options.AddEnumXmlDescriptionTransformer(xmlPath);
});
```