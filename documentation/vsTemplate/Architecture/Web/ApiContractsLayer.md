---
title: RaTemplate.Api.Contracts
sidebar_position: 1
---

`Api.Contracts` project is a valid pattern, often used in different scenarios:

* **Client SDKs**:
If you were building a client SDK (e.g., a .NET client library to consume your own API), you could put the request/response DTOs in a shared `Contracts` library.
This would allow both the API and the client to share the same data models without the client needing to reference your entire `Application` project.
* **Microservices Communication**:
In a microservices environment, a shared `Contracts` library can contain the DTOs and events that are exchanged between different services.

## 🏛️ Architectural Principles

*   **Independence**: This project has **zero dependencies** on any other project within the solution.
It does not know about the `Application`, `Infrastructure`, or `Api` layers. This is the most critical rule of the Domain layer.