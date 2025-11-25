---
title: Example
sidebar_position: 5
---

This document provides instructions for running the example project, which demonstrates a complete application built using the RA.Utilities ecosystem.

## 🚀 Getting Started

To run the example project, navigate to the `example` directory from the root of the repository and execute the application.

```bash
cd example
dotnet run
```

App will run on `http://localhost:5039`.
Then go to `http://localhost:5039/openapi-ui` to test endpoints. 

## Authorization
The API endpoints are secured using JWT Bearer token authorization.
To access protected routes, you must provide a valid token in the `Authorization` header.

You can generate token from this [site](https://10015.io/tools/jwt-encoder-decoder#google_vignette)

To generate the token, use the following configuration: 
**1. Payload (Decoded)**
The token must contain the following claims.
You can copy and paste this JSON into the "Payload" section of the JWT tool.

```json
{
  "iss": "your-issuer",
  "aud": "your-audience",
  "scope": [
    "todo:create",
    "todo:edit"
  ],
  "sub": "1234567890",
  "name": "John Doe",
  "iat": 1516239022
}
```

**2. Signature**
Ensure the signature is generated using the `HS256` algorithm and the following secret key.
This key must be Base64 encoded. `wJalv8d9L/K2A/V3q+p4bZ5cQ8eF7gR6iT3sN0mY5Zg=`