namespace App.Server.Notification.Application.Domain.Entities.JsonEntities;

/// <summary>
/// Marker interface for JSON entities.
/// </summary>
/// <remarks>
/// JSON entities are entities that are serialized and deserialized to and from JSON.
/// If marked with this interface, entities will be automatically mapped to JSON in the database.
/// </remarks>
public interface IJsonEntity;
