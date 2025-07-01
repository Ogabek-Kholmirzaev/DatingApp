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
            var userId = resultContext.HttpContext.User.GetUserId();
            var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                await unitOfWork.CompleteAsync();
            }
        }
    }
}