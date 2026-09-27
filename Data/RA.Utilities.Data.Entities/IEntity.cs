namespace RA.Utilities.Data.Entities;

/// <summary>
/// Marker interface implemented by all entities (see <see cref="CoreEntity{TKey}"/>).
/// It defines no members; its purpose is to let generic abstractions, such as the repository interfaces, constrain their type parameters to entities.
/// </summary>
public interface IEntity;
