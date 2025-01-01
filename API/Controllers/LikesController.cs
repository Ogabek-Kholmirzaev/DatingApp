using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUserLikes([FromQuery] LikesParams @params)
    {
        @params.UserId = User.GetUserId();

        var users = await likesRepository.GetUserLikes(@params);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }

    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId)
        {
            return BadRequest("You cannot like yourself");
        }

        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

        if (existingLike == null)
        {
            var userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };

            likesRepository.AddLike(userLike);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }
        
        if (await likesRepository.SaveChangesAsync())
        {
            return Ok();
        }

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }
}
