using System.Net;
using System.Text.Json;
using FinBineBackend.BackEnd.Logs.Services;

namespace FinBineBackend.BackEnd.Middleware
{
    // This class wraps around every incoming web request. If anything
    // inside the request throws an unhandled exception, it gets caught
    // here, logged, and turned into a clean error response — instead
    // of the app crashing or leaking a raw error to whoever called it.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, BackEndLoggingService backEndLogger)
        {
            try
            {
                // Let the request continue on to whatever it was actually
                // trying to do (a controller, etc.)
                await _next(context);
            }
            catch (Exception ex)
            {
                // Something broke. Log it using the method that already
                // existed but was never being called.
                backEndLogger.LogUnhandledException(ex);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorResponse = new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}