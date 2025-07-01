using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUserLikes([FromQuery] LikesParams @params)
    {
        @params.UserId = User.GetUserId();

        var users = await unitOfWork.LikesRepository.GetUserLikes(@params);

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

        var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);
        if (existingLike == null)
        {
            var userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };

            await unitOfWork.LikesRepository.AddLikeAsync(userLike);
        }
        else
        {
            unitOfWork.LikesRepository.DeleteLike(existingLike);
        }

        return await unitOfWork.CompleteAsync() ? Ok() : BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }
}
