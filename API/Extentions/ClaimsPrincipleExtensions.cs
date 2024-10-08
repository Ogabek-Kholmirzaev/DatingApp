using System.Security.Claims;

namespace API.Extentions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return username ?? throw new Exception("Cannot get username from token");
    }
}