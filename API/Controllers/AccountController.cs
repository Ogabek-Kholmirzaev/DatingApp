using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
    UserManager<AppUser> userManager,
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

        var user = mapper.Map<AppUser>(registerDto);
        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return new UserDto
        {
            Username = user.UserName!,
            KnowsAs = user.KnownAs,
            Token = await tokenService.CreateTokenAsync(user),
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        var user = await userManager.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

        if (user == null)
        {
            return Unauthorized("Invalid username");
        }

        return new UserDto
        {
            Username = user.UserName!,
            KnowsAs = user.KnownAs,
            Token = await tokenService.CreateTokenAsync(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedEmail == username.ToUpper());
    }
}
