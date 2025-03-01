using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
    DataContext context,
    ITokenService tokenService,
    IMapper mapper)
    : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is already taken");
        }

        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerDto);

        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        // user.PasswordSalt = hmac.Key;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName!,
            KnowsAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await context.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

        if (user == null)
        {
            return Unauthorized("Invalid username");
        }

        // using var hmac = new HMACSHA512(user.PasswordSalt);
        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i])
        //     {
        //         return Unauthorized("Invalid password");
        //     }
        // }

        return new UserDto
        {
            Username = user.UserName!,
            KnowsAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.NormalizedEmail == username.ToUpper());
    }
}
