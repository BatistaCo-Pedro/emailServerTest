namespace App.Server.Notification.Tests.Integration;

// TODO: Replace with actual tests or remove.
public class ExampleTest : IClassFixture<TestWebApplicationFactory>
{
    private readonly IUnitOfWork _unitOfWork;

    public ExampleTest(TestWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    [Fact(Skip = "ToDo: Does not work.")]
    public void Test1() { }
}
