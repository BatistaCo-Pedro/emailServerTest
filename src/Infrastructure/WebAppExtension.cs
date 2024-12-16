using Microsoft.AspNetCore.Builder;

namespace App.Server.Notification.Infrastructure;

/// <summary>
/// Extensions class containing web application methods.
/// </summary>
public static class WebAppExtension
{
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseHangfireDashboard();
        app.MapHangfireDashboard();

        return app;
    }
}
