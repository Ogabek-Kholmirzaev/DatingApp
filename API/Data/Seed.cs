using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class Seed
{
    public static async Task DbMigrateAndSeedUsersAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        await dataContext.Database.MigrateAsync();

        await dataContext.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
        await dataContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Connections\"");

        if (!await dataContext.Users.AnyAsync())
        {
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            if (users != null)
            {
                var roles = new List<AppRole>
                {
                    new() { Name =  "Member" },
                    new() { Name =  "Admin" },
                    new() { Name =  "Moderator" }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                foreach (var user in users)
                {
                    user.Photos.First().IsApproved = true;
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Member");
                }

                var admin = new AppUser
                {
                    UserName = "admin",
                    KnownAs = "Admin",
                    Gender = "",
                    City = "",
                    Country = ""
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
            }
        }
    }
}