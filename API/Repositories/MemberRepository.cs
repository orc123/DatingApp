using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<MemberDto?> GetMemberByIdAsync(string id)
    {
        return await context.Members
            .Select(x => new MemberDto
            {
                Id = x.Id,
                DateOfBirth = x.DateOfBirth,
                ImageUrl = x.ImageUrl,
                DisplayName = x.DisplayName,
                Created = x.Created,
                LastActive = x.LastActive,
                Gender = x.Gender,
                Description = x.Description,
                City = x.City,
                Country = x.Country,
                Photos = x.Photos.Select(x => new PhotoDto()
                {
                    Id = x.Id,
                    Url = x.Url,
                    MemberId = x.MemberId,
                    PublicId = x.PublicId,
                }).ToList()
            }).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<MemberDto?> GetMemberForUpdateAsync(string memberId)
    {
        return await context.Members
            .Include(x => x.User)
            .Select(x => new MemberDto
            {
                Id = x.Id,
                DateOfBirth = x.DateOfBirth,
                ImageUrl = x.ImageUrl,
                DisplayName = x.DisplayName,
                Created = x.Created,
                LastActive = x.LastActive,
                Gender = x.Gender,
                Description = x.Description,
                City = x.City,
                Country = x.Country,
                User = new UserDto()
                {
                    Id = x.User.Id,
                    DisplayName = x.User.DisplayName,
                    Email = x.User.Email,
                    ImageUrl = x.User.ImageUrl,
                    Token = ""
                }
            }).SingleOrDefaultAsync(x => x.Id == memberId);
    }

    public async Task<IReadOnlyList<MemberDto>> GetMembersAsync()
    {
        return await context.Members.Select(x => new MemberDto
        {
            Id = x.Id,
            DateOfBirth = x.DateOfBirth,
            ImageUrl = x.ImageUrl,
            DisplayName = x.DisplayName,
            Created = x.Created,
            LastActive = x.LastActive,
            Gender = x.Gender,
            Description = x.Description,
            City = x.City,
            Country = x.Country,
        })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<PhotoDto>> GetPhotosForMemberAsync(string memberId)
    {
       return await context.Members.Where(x => x.Id == memberId)
                            .SelectMany(x => x.Photos)
                            .Select(y => new PhotoDto
                            {
                                Id = y.Id,
                                Url = y.Url,
                                MemberId = y.MemberId,
                                PublicId = y.PublicId,
                            })
                            .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(string memberId, MemberUpdateDto memberUpdateDto)
    {
        var member = await context.Members.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == memberId);
        if (member == null)
        {
            return false;
        }

        member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDto.Description ?? member.Description;
        member.City = memberUpdateDto.City ?? member.City;
        member.Country = memberUpdateDto.Country ?? member.Country;

        member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

        context.Members.Update(member);
        bool result = await context.SaveChangesAsync() > 0;
        return result;
    }
}
