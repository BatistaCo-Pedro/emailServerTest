namespace App.Server.Notification.Infrastructure.Messaging.DomainEvents.Consumers;

public class BaseConsumer<T, TOutput>(IEventHandler<T, TOutput> t) : IConsumer<T>
    where T : class, IEventMessage
    where TOutput : class
{
    public async Task Consume(ConsumeContext<T> context)
    {
        var output = await t.Handle(context.Message);
        await context.RespondAsync(output);
    }
}

public class BaseConsumer<T>(IEventHandler<T> eventHandler) : IConsumer<T>
    where T : class, IEventMessage
{
    public async Task Consume(ConsumeContext<T> context)
    {
        await eventHandler.Handle(context.Message);
    }
}
