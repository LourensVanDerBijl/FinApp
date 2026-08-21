using FinBineBackend.AdminAuthentication.Services;
using FinBineBackend.AdminAuthentication.Logs.Services;
using FinBineBackend.BackEnd.Logs.Services;
using FinBineBackend.BackEnd.Middleware;
using FinBineBackend.LoggingLogic.LogDbWrite.Data;
using FinBineBackend.LoggingLogic.LogDbWrite.Services;
using FinBineBackend.LoggingLogic.Logs.Services;
using FinBineBackend.PlatformHealth.Logs.Services;
using FinBineBackend.PlatformHealth.Services;
using FinBineBackend.PlatformHealth.BackgroundWorkers;
using FinBineBackend.UserAccRegistration.Data;
using FinBineBackend.UserAccRegistration.Services;
using FinBineBackend.UserAccRegistration.Logs.Services;
using FinBineBackend.UserLoginAuthentication.Services;
using FinBineBackend.UserLoginAuthentication.Logs.Services;
using FinBineBackend.UserGroupRegistration.Services;
using FinBineBackend.UserGroupRegistration.Data;
using FinBineBackend.UserGroupRegistration.Logs.Services;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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
// User Account Registration
// ------------------------------------------------------------

// Users table — the minimal Postgres anchor row (see UserDbRecord).
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FinBineDatabase")
    )
);

// fb_users Firestore collection — sequential ID generation + document
// read/write/delete (delete is used for rollback on a failed registration).
builder.Services.AddScoped<UserFirestoreService>();

// Orchestrates a full registration: Firebase Auth -> Firestore -> Postgres,
// with automatic all-or-nothing rollback if any step fails.
builder.Services.AddScoped<UserRegistrationService>();

// ------------------------------------------------------------
// User Login Authentication
// ------------------------------------------------------------

// Reads fb_users (not fb_admin_users) to confirm a logged-in Firebase
// account actually has a FinBine profile, and updates last_activity on
// a successful login.
builder.Services.AddScoped<UserFirestoreLoginService>();

// Verifies the login token and applies the fb_users membership + account
// standing checks — same shape as AdminAuthService, different collection.
builder.Services.AddScoped<UserLoginAuthService>();

// ------------------------------------------------------------
// User Group Registration
// ------------------------------------------------------------

// Groups table — the minimal Postgres anchor row (see GroupDbRecord),
// same idea as UserDbContext.
builder.Services.AddDbContext<GroupDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FinBineDatabase")
    )
);

// fb_groups Firestore collection — sequential ID generation, document
// read/write/delete (delete is used for rollback), plus updating the
// owner's own fb_users document once their group exists.
builder.Services.AddScoped<GroupFirestoreService>();

// Orchestrates group creation: verify token -> validate fields -> look
// up the owner's profile -> write fb_groups -> update fb_users -> write
// the Postgres anchor row, with automatic all-or-nothing rollback if
// any step fails.
builder.Services.AddScoped<UserGroupRegistrationService>();

// ------------------------------------------------------------
// Log Database (the central collector every logging service below
// writes into, via LogQueue — see LoggingLogic.LogDbWrite)
// ------------------------------------------------------------

builder.Services.AddDbContext<LogDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("FinBineDatabase")
    )
);

builder.Services.AddScoped<LogDbWriteService>();

// ------------------------------------------------------------
// Logging Services
//
// Every one of these writes into the same central LogQueue above,
// regardless of which folder it lives in — grouped together here purely
// so it's obvious at a glance that every feature's logging is wired up.
// ------------------------------------------------------------

builder.Services.AddScoped<AuthLoggingService>();
builder.Services.AddScoped<BackEndLoggingService>();
builder.Services.AddScoped<PlatformHealthLoggingService>();
builder.Services.AddScoped<LoggingSystemLoggingService>();
builder.Services.AddScoped<UserRegistrationLoggingService>();
builder.Services.AddScoped<UserLoginLoggingService>();
builder.Services.AddScoped<UserGroupRegistrationLoggingService>();

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
// Firebase initialization
//
// This MUST happen here, once, at startup — not as a side effect of
// any one feature's constructor. Previously this only ran when
// AdminAuthService was instantiated, meaning User Registration/Login
// would fail with a NullReferenceException if no admin had logged in
// yet since the backend last started. Every service that calls
// FirebaseAuth.DefaultInstance (AdminAuthService, UserRegistrationService,
// UserLoginAuthService, etc.) now relies on this having already run.
// ------------------------------------------------------------
if (FirebaseApp.DefaultInstance == null)
{
    var firebaseCredential = CredentialFactory.FromFile(
        "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json",
        "service_account"
    );

    FirebaseApp.Create(new AppOptions
    {
        Credential = firebaseCredential
    });
}

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