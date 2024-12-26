using System.Security.Claims;

namespace API.Extentions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name);

        return username ?? throw new Exception("Cannot get username from token");
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(id, out int userId))
        {
            return userId;
        }

        throw new Exception("Id is null or cannot parse to integer");
    }
}