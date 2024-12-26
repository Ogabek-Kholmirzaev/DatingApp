using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var username = resultContext.HttpContext.User.GetUsername();
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetUserByUsernameAsync(username);

            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                await userRepository.SaveAllAsync();
            }
        }
    }
}