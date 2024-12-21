namespace App.Server.Notification.Infrastructure.Messaging.DomainEvents;

/// <summary>
/// Wrapper around <see cref="IEventHandler{T,TOutput}"/> to consume messages by a mediator.
/// </summary>
/// <param name="eventHandler">The event handler to call.</param>
/// <typeparam name="TEventMessage">The type of the event message.</typeparam>
/// <typeparam name="TResponse">The type of the response to send.</typeparam>
/// <remarks>
/// This wrapper exists to maintain the Application layer clean and free of infrastructure concerns.
/// </remarks>
public class Consumer<TEventMessage, TResponse>(
    IEventHandler<TEventMessage, TResponse> eventHandler
) : IConsumer<TEventMessage>
    where TEventMessage : class, IEventMessage
    where TResponse : class
{
    public async Task Consume(ConsumeContext<TEventMessage> context)
    {
        var output = await eventHandler.Handle(context.Message);
        await context.RespondAsync(output);
    }
}

/// <summary>
/// Wrapper around <see cref="IEventHandler{T,TOutput}"/> to consume messages by a mediator.
/// </summary>
/// <param name="eventHandler">The event handler to call.</param>
/// <typeparam name="TEventMessage">The type of the event message.</typeparam>
/// /// <remarks>
/// This wrapper exists to maintain the Application layer clean and free of infrastructure concerns.
/// </remarks>
public class Consumer<TEventMessage>(IEventHandler<TEventMessage> eventHandler)
    : IConsumer<TEventMessage>
    where TEventMessage : class, IEventMessage
{
    public async Task Consume(ConsumeContext<TEventMessage> context)
    {
        await eventHandler.Handle(context.Message);
    }
}
