using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers()
    {
        var members = await memberRepository.GetMembersAsync();
        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMember(string id)
    {
        var member = await memberRepository.GetMemberByIdAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMemberPhotos(string id)
    {
        return Ok(await memberRepository.GetPhotosForMemberAsync(id));
    }
    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
        var memberId = User.GetMemberId();

        var memberDto = await memberRepository.GetMemberByIdAsync(memberId);

        if (memberDto == null)
        {
            return BadRequest("Coulb not get member");
        }

        var result = await memberRepository.UpdateAsync(memberId , memberUpdateDto);

        if (result)
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Failed to update member");
        }
    }
}
