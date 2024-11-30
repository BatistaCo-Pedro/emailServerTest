// Don't turn this into a global usings - Infrastructure is only referenced for service registrations.
using App.Server.Notification.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString =
    configuration.GetConnectionString("Default")
    ?? throw new ApplicationException("Connection string not found.");

builder.AddSerilog();

services.AddPresentation();

// Infrastructure services
services.AddDatabase(connectionString).AddBackgroundQueue(connectionString);

// Application services
services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();

public partial class Program { }
