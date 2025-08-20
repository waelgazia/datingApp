using System;
using System.Net;
using System.Text.Json;
using DatingApp.API.Exceptions;

namespace DatingApp.API.Middlewares;

public class ExceptionMiddleware(
    RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = env.IsDevelopment()
                ? new ApiException(httpContext.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiException(httpContext.Response.StatusCode, ex.Message, "Internal server error");

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var responseJson = JsonSerializer.Serialize(response, options);
            await httpContext.Response.WriteAsync(responseJson);
        }
    }
}
