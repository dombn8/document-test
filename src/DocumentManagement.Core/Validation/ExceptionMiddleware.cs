using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DocumentManagement.Core.Validation
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger Logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            object? errors = null;

            if (exception is ValidationException re)
            {
                statusCode = (int)re.StatusCode;

                if (re.Message != null & re.Message is string)
                {
                    errors = new[] { re.Message };
                }
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errors = "An internal server error has occured.";
            }

            if (exception.TargetSite != null)
                Logger.LogError(
                    $"{errors} - {exception.Source} - {exception.Message} - {exception.StackTrace} - {exception.TargetSite.Name}");

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(errors));
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
