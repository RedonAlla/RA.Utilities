```powershell
Namespace: RA.Utilities.OpenApi.SchemaTransformers
```

The `ValueObjectSchemaTransformer` documents the validation constraints enforced by the value objects from [`RA.Utilities.Core.ValueObjects`](../../../core/RA.Utilities.Core.ValueObjects/index.mdx) directly in the generated OpenAPI schema.

Value objects validate and normalize their input in the constructor, so `Microsoft.AspNetCore.OpenApi` cannot see their rules. Without this transformer, `Email`, `Currency`, `SSN`, and `IBAN` are emitted as property-less objects that only carry the XML summary, and API consumers have no way to know what a valid value looks like.

### 🎯 What it documents

| Value object | OpenAPI schema |
| ------------ | -------------- |
| `Email` | `type: string`, `format: email`, pattern, `minLength: 1`, `maxLength: 254` |
| `Currency` | `type: string`, pattern, `minLength: 3`, `maxLength: 3` |
| `SSN` | `type: string`, pattern, `minLength: 10`, `maxLength: 10` |
| `IBAN` | `type: string`, `format: iban`, pattern, `minLength: 13`, `maxLength: 34` |
| `Money` | `type: object`, description, and an `{ "amount": 19.99, "currency": "EUR" }` example |

Each string schema also receives a description and an example value. The patterns mirror the rules enforced by the value objects:

| Value object | Pattern |
| ------------ | ------- |
| `Email` | `^[^@\s]+@[^@\s]+\.[^@\s]+$` |
| `Currency` | `^[A-Z]{3}$` |
| `SSN` | `^[A-Z][0-9]{8}[A-Z]$` |
| `IBAN` | `^[A-Z]{2}[0-9]{2}[A-Z0-9]{9,30}$` |

:::note
The `IBAN` pattern describes the canonical, normalized form. The value object also accepts grouping separators (spaces, apostrophes, backslashes, and hyphens) and upper-cases the input, so `"gb82 west 1234-5698-7654-32"` is valid and is stored as `"GB82WEST12345698765432"`.
:::

### ⚙️ How it works

For every schema whose CLR type is one of the supported value objects, the transformer:

1. Replaces the generated object schema (if any) with a `string` schema (or keeps `Money` as an object).
2. Applies the `format`, `pattern`, `minLength`, `maxLength`, description, and example.
3. Leaves every other schema untouched.

The transformer is idempotent, so it is safe to register it more than once.

### 🚀 How to use it

`AddDefaultsDocumentTransformer()` registers the transformer automatically:

```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    // highlight-next-line
    options.AddDefaultsDocumentTransformer();
});
```

If you register transformers individually, add it explicitly:

```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    // highlight-next-line
    options.AddValueObjectSchemaTransformer();
});
```

### 📄 Example output

For a request model such as:

```csharp
public sealed record CreateUserRequest
{
    public required Email Email { get; init; }
    public required Currency Currency { get; init; }
    public required SSN Ssn { get; init; }
    public required IBAN Iban { get; init; }
    public required Money Balance { get; init; }
}
```

the generated OpenAPI components become:

```json
{
  "Email": {
    "type": "string",
    "format": "email",
    "pattern": "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$",
    "minLength": 1,
    "maxLength": 254,
    "description": "A validated and normalized email address. ...",
    "examples": ["user@example.com"]
  },
  "Currency": {
    "type": "string",
    "pattern": "^[A-Z]{3}$",
    "minLength": 3,
    "maxLength": 3,
    "description": "A validated and normalized ISO 4217 currency code. ...",
    "examples": ["EUR"]
  },
  "IBAN": {
    "type": "string",
    "format": "iban",
    "pattern": "^[A-Z]{2}[0-9]{2}[A-Z0-9]{9,30}$",
    "minLength": 13,
    "maxLength": 34,
    "description": "A validated and normalized International Bank Account Number (ISO 13616). ...",
    "examples": ["GB82WEST12345698765432"]
  },
  "Money": {
    "type": "object",
    "required": ["amount", "currency"],
    "properties": {
      "amount": { "type": ["number", "string"], "format": "double" },
      "currency": { "$ref": "#/components/schemas/Currency" }
    },
    "description": "A monetary amount in a specific currency. ...",
    "examples": [{ "amount": 19.99, "currency": "EUR" }]
  }
}
```

:::info
The constraints are documentation only. The value objects still validate and normalize input on the server; the OpenAPI schema helps clients send valid values and helps generated SDKs expose the correct shape.
:::
