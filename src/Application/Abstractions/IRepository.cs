namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Represents a repository.
/// </summary>
public interface IRepository;

/// <summary>
/// Generic repository interface.
/// </summary>
/// <typeparam name="T">The generic type of the interface.</typeparam>
public interface IRepository<T> : IRepository;
