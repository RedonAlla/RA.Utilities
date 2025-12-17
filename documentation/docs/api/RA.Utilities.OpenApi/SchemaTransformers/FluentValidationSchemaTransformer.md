```powershell
Namespace: RA.Utilities.OpenApi.SchemaTransformers
```

The `FluentValidationSchemaTransformer` is a powerful utility that serves a single, clear purpose: **to automatically enrich your OpenAPI (Swagger) documentation by translating your [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) rules into schema constraints**.

In simpler terms, it reads the validation rules you've already written for your request models and applies them directly to your API documentation.
For example:
  * `NotNull()` or `NotEmpty()` will mark a property as required.
  * `Length(min, max)` sets the `minLength` and `maxLength` schema properties.
  * `Matches("regex")` sets the `pattern` property.
  * `GreaterThan(10)` sets the `minimum` and `exclusiveMinimum` properties.
  * `EmailAddress()` sets the `format` to `"email"`.



### 🎯 Key Purpose and Benefits

1. **Single Source of Truth**: This is the most significant benefit.
Your [`FluentValidation`](https://docs.fluentvalidation.net/en/latest/) classes become the single source of truth for validation.
You no longer need to duplicate this logic with data attributes like `[Required]` or `[MaxLength]` on your models just to get them to show up in the Swagger UI.
This reduces code duplication and the risk of your documentation becoming out-of-sync with your actual validation logic.
2. **Improved API Contract**: By automatically documenting these rules, you provide a much richer and more accurate API contract to your consumers.
Developers using your API can immediately see constraints like required fields, length limits, and data formats directly in the documentation, leading to a better developer experience and fewer invalid requests.
3. **Automated and Boilerplate-Free**: The transformer works automatically. Once registered, it inspects your models and their corresponding validators, eliminating the need for developers to manually annotate every property on every request model.

### ⚙️ How It Works
For each model in your API:

1. It finds the registered IValidator for that model type.
2. It inspects the rules for each property.
3. It then applies the corresponding OpenAPI schema constraint. For example:
  * `NotNull()` or `NotEmpty()` will mark a property as `required`.
  * `Length(min, max)` sets the `minLength` and `maxLength` schema properties.
  * `Matches("regex")` sets the `pattern` property.
  * `GreaterThan(10)` sets the `minimum` and `exclusiveMinimum` properties.
  * `EmailAddress()` sets the `format` to `"email"`.
  * `CreditCard()` sets the `format` to `"credit-card"`.

### 🚀 How to use it

Simply call the `AddFluentValidationRules()` extension method when configuring your OpenAPI services.

```csharp
// In Program.cs
builder.Services.AddOpenApi(options =>
{
    // ... other configurations
    // highlight-next-line
    options.AddFluentValidationRules();
});
```