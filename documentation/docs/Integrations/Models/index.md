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

[`QueryParams`](./QueryParams) is the data vocabulary of every request: the generated code for [`[QueryParameters]`](../Attributes/QueryParametersAttribute.md) classes and hand-written [`IQueryStringRequest`](../Abstractions/IQueryStringRequest.md) implementations both produce `QueryParams` collections, which [`QueryUtilities`](../Utilities/QueryUtilities.md) then URL-encodes into the final request URI.

For integration configuration, see the [`Options`](../Options/index.mdx) namespace — [`BaseApiSettings<T>`](../Options/BaseApiSettings.md) is the validated settings base class that binds base URL, strongly-typed `Actions`, proxy flag, and timeout from configuration.

<br />
<br />

import DocCardList from '@theme/DocCardList';

<DocCardList />
