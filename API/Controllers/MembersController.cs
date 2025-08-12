using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class MembersController(IMemberRepository memberRepository, IPhotoService photoService, IPhotoRepository photoRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MemberDto>>> GetMembers([FromQuery]MemberParams memberParams)
    {

        memberParams.CurrentMemberId = User.GetMemberId();
        var members = await memberRepository.GetMembersAsync(memberParams);
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
            return BadRequest("Could not get member");
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

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm]IFormFile file)
    {
        var member = await memberRepository.GetMemberForUpdateAsync(User.GetMemberId());

        if (member == null)
        {
            return BadRequest("Cannot update member");
        }

        var result = await photoService.UploadPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photoDto = new PhotoDto
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            MemberId = User.GetMemberId(),
        };

        PhotoDto? resultDto = await photoRepository.AddPhotoAsync(photoDto);
        if (resultDto == null)
        {
            return BadRequest("Problem adding photo");
        }

        return resultDto;
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhotoAsync(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdateAsync(User.GetMemberId());

        if (member == null)
        {
            return BadRequest("Cannot get member from token");
        }

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
        if (member.ImageUrl == photo?.Url || photo == null)
        {
            return BadRequest("Cannot set this as main Image");
        }
        var result = await photoRepository.SetMainPhotoAsync(member.Id, photoId);

        if (result > 0)
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Problem with set main photo");
        }
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var member = await memberRepository.GetMemberForUpdateAsync(User.GetMemberId());

        if (member == null)
        {
            return BadRequest("Cannot get member from token");
        }

        var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
        if (member.ImageUrl == photo?.Url || photo == null)
        {
            return BadRequest("This photo cannot be deleted!");
        }

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
        }
        var resultFromDb = await photoRepository.DeletePhoto(member.Id, photoId);

        if (resultFromDb > 0)
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Problem deleting the photo");
        }

    }
}
