# Release Notes

## 1.0.1-preview.6.3

### Bug Fixes
- **Build:** Resolved `NU1510` warning by removing the redundant `Microsoft.Extensions.Logging.Abstractions` package reference. This dependency is already included via the `Microsoft.AspNetCore.App` framework reference.
- **Build:** Fixed `MSB4025` project load error caused by an invalid character (BOM) at the beginning of the `.csproj` file.

## 1.0.0-preview.6.3
- Initial release of the `RA.Utilities.Api.Middlewares` package.
