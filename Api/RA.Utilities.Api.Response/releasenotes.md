# RA.Utilities.Api.Response Release Notes

## Version 10.0.0
![Date Badge](https://img.shields.io/badge/Publish-Unreleased-lightblue?logo=fastly&logoColor=white)

### ✨ New Features & Improvements

*   **Package revived**: The response models consolidated into `RA.Utilities.Api` in v10.0.6 are published again as the standalone `RA.Utilities.Api.Response` package. The `RA.Utilities.Api.Response` namespace and all public types are unchanged.
*   `RA.Utilities.Api` now depends on this package, so existing consumers receive the types transitively; projects that only need response models can reference this package directly.