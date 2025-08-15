using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class LikesController(ILikeRepository likeRepository) : BaseApiController
{
    [HttpPost("{targetMemberId}")]
    public async Task<ActionResult> ToggleLike(string targetMemberId)
    {
        var sourceMemberId = User.GetMemberId();
        if (sourceMemberId == targetMemberId)
        {
            return BadRequest("You cannot like yourself");
        }

        var existingLike = await likeRepository.GetMemberLike(sourceMemberId, targetMemberId);

        var like = new MemberLikeDto
        {
            SourceMemberId = sourceMemberId,
            TargetMemberId = targetMemberId
        };

        if (existingLike == null)
        {
            var result = await likeRepository.AddLikeAsync(like);
            if (result > 0)
            {
                return Ok(like);
            }
            else
            {
                return BadRequest("Failed to update like");
            }
        }
        else 
        {
            var deleteResult = await likeRepository.DeleteLikeAsync(like);
            if (deleteResult > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to update like");
            }

        }
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
    {
        var userId = User.GetMemberId();
        return Ok(await likeRepository.GetCurrentMemberLikeIds(userId));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MemberDto>>> GetMemberLikes([FromQuery]LikesParam likesParam)
    {
        likesParam.MemberId = User.GetMemberId();
        var members = await likeRepository.GetMemberLikes(likesParam);
        return Ok(members);
    }
}
