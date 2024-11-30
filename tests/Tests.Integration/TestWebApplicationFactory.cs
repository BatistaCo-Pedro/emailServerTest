namespace App.Server.Notification.Tests.Integration;

public partial class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithUsername("postgresql")
        .WithPassword("postgresql")
        .WithDatabase("HangfireTestingDb")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Remove(services.Single(x => x.ServiceType == typeof(NotificationDbContext)));
            services.Remove(services.Single(x => x.ServiceType == typeof(IUnitOfWork)));

            services.AddDatabase(_dbContainer.GetConnectionString());
        });

        // Seed database
        // var seeder = getSeeder
    }

    /// <summary>
    /// Initializes the database container.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    /// <summary>
    /// Disposes the database container.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
