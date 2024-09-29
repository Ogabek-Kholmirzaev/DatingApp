using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class Seed
{
    public static async Task SeedUsers(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        await dataContext.Database.MigrateAsync();

        if (!await dataContext.Users.AnyAsync())
        {
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            if (users != null)
            {
                foreach (var user in users)
                {
                    using var hmac = new HMACSHA512();

                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt = hmac.Key;
                }

                await dataContext.Users.AddRangeAsync(users);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}