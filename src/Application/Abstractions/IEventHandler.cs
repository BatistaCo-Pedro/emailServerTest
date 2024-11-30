namespace App.Server.Notification.Application.Abstractions;

/// <summary>
/// Interface for handlers.
/// </summary>
/// <typeparam name="T">The type of the event.</typeparam>
/// <typeparam name="TOutput">The type of the output.</typeparam>
public interface IEventHandler<in T, TOutput>
    where T : class, IEventMessage
    where TOutput : class
{
    public Task<TOutput> Handle(T eventMessage);
}

/// <summary>
/// Interface for handlers.
/// </summary>
/// <typeparam name="T">The type of the event</typeparam>
public interface IEventHandler<in T>
    where T : class, IEventMessage
{
    public Task Handle(T eventMessage);
}