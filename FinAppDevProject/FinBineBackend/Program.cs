using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FinBineBackend.AdminAuthentication.Services; // ✅ import your service namespace

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// ✅ Register your AdminAuthService so DI can inject it into AdminAuthController
builder.Services.AddScoped<AdminAuthService>();

// ✅ Configure CORS so your Vue frontend (running on https://localhost:5173) can call the backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173") // Vue dev server (HTTPS only)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // ✅ allow cookies/tokens if needed
        });
});

var app = builder.Build();

// ✅ Enforce HTTPS redirection
app.UseHttpsRedirection();

// ✅ Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();

// ✅ Map controllers
app.MapControllers();

app.Run();
