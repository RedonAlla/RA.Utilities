# Release Notes

## Version 1.0.0-preview.6.3

### Initial Release

This is the initial release of `RA.Utilities.Data.Entities`, a library providing core abstractions for data models.

#### Features

*   **`IEntity<T>`**: A generic interface defining a contract for entities with a strongly-typed primary key.
*   **`BaseEntity<T>`**: An abstract base class implementing `IEntity<T>` with common auditing properties (`Id`, `CreatedDate`, `UpdatedDate`).
*   **`BaseEntity`**: A non-generic convenience base class that defaults the primary key to type `long`.