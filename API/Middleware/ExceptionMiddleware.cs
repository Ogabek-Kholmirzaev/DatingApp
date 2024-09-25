using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger,
    IHostEnvironment hostEnvironment)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = hostEnvironment.IsDevelopment()
                ? new ApiException(httpContext.Response.StatusCode, exception.Message, exception.StackTrace)
                : new ApiException(httpContext.Response.StatusCode, exception.Message, "Internal server error");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json);
        }
    }
}
