using FinBineBackend.AdminAuthentication.Services;
using FinBineBackend.AdminAuthentication.Logs.Services;
using FinBineBackend.BackEnd.Logs.Services;
using FinBineBackend.BackEnd.Middleware;
using FinBineBackend.LoggingLogic.LogDbWrite.Data;
using FinBineBackend.LoggingLogic.LogDbWrite.Services;
using FinBineBackend.PlatformHealth.Logs.Services;
using FinBineBackend.PlatformHealth.Services;
using FinBineBackend.LoggingLogic.Logs.Services;
using FinBineBackend.PlatformHealth.BackgroundWorkers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Services
// ------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddHttpClient();

// ------------------------------------------------------------
// Admin Authentication
// ------------------------------------------------------------

builder.Services.AddScoped<AdminAuthService>();

// ------------------------------------------------------------
// Log Database
// ------------------------------------------------------------

builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FinBineDatabase")
    )
);

builder.Services.AddScoped<LogDbWriteService>();

// ------------------------------------------------------------
// Logging Services
// ------------------------------------------------------------

builder.Services.AddScoped<AuthLoggingService>();
builder.Services.AddScoped<BackEndLoggingService>();
builder.Services.AddScoped<PlatformHealthLoggingService>();
builder.Services.AddScoped<LoggingSystemLoggingService>();

// ------------------------------------------------------------
// Platform Health
// ------------------------------------------------------------

builder.Services.AddScoped<PlatformHealthCheckService>();
builder.Services.AddHostedService<PlatformHealthBackgroundWorker>();

// ------------------------------------------------------------
// Background Workers
// ------------------------------------------------------------

builder.Services.AddHostedService<LogDbBackgroundWorker>();

// ------------------------------------------------------------
// CORS
// ------------------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ------------------------------------------------------------
// Build application
// ------------------------------------------------------------

var app = builder.Build();

// ------------------------------------------------------------
// Middleware
// ------------------------------------------------------------

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// ------------------------------------------------------------
// Logging lifecycle events
// ------------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<BackEndLoggingService>();
    try
    {
        logger.LogBackendStarted();
        logger.LogBackendStartedSuccessfully();
    }
    catch (Exception ex)
    {
        logger.LogBackendStartupFailed(ex);
    }

    var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStopping.Register(() =>
    {
        logger.LogBackendStopping();
    });
    lifetime.ApplicationStopped.Register(() =>
    {
        logger.LogBackendStopped();
    });
}

// ------------------------------------------------------------
// Run
// ------------------------------------------------------------

app.Run();