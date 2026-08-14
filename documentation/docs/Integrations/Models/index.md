---
title: Models
sidebar_position: 5
---

```bash
Namespace: RA.Utilities.Integrations.Models
```

The `RA.Utilities.Integrations.Models` namespace contains the data models used to build requests and configure integrations:

| Class | Purpose |
|---|---|
| [`QueryParams`](./QueryParams) | An ordered, URL-encodable collection of key-value pairs used to build query strings — the return type of `IQueryStringRequest.QueryStringValues()`. |
| [`BaseHttpClientSettings<T>`](./BaseHttpClientSettings) | A ready-to-use settings base class implementing `IIntegrationSettings` with a required base URL, strongly-typed `Actions`, proxy flag, and timeout. |

The query string side is the data vocabulary of every request: the generated code for [`[QueryParameters]`](../Attributes/QueryParametersAttribute.md) classes and hand-written [`IQueryStringRequest`](../Abstractions/IQueryStringRequest.md) implementations both produce [`QueryParams`](./QueryParams) collections, which [`QueryUtilities`](../Utilities/QueryUtilities.md) then URL-encodes into the final request URI.

The configuration side complements the [`Options`](../Options/index.mdx) namespace: [`BaseHttpClientSettings<T>`](./BaseHttpClientSettings) is the concrete, minimal-validation counterpart of the abstract, fully-validated [`BaseApiSettings<T>`](../Options/BaseApiSettings.md).

<br />
<br />

import DocCardList from '@theme/DocCardList';

<DocCardList />
