using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories;

public class LikeRepository(AppDbContext context) : ILikeRepository
{
    public async Task<int> AddLikeAsync(MemberLikeDto likeDto)
    {
        var like = new MemberLike
        {
            SourceMemberId = likeDto.SourceMemberId,
            TargetMemberId = likeDto.TargetMemberId
        };

        context.Likes.Add(like);

        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<int> DeleteLikeAsync(MemberLikeDto likeDto)
    {
        var like = new MemberLike
        {
            SourceMemberId = likeDto.SourceMemberId,
            TargetMemberId = likeDto.TargetMemberId
        };

        context.Likes.Remove(like);

        var result = await context.SaveChangesAsync();
        return result;
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        return await context.Likes
            .Where(x => x.SourceMemberId == memberId)
            .Select(x => x.TargetMemberId)
            .ToListAsync();
    }

    public async Task<MemberLikeDto?> GetMemberLike(string sourceMemberId, string targetMemberId)
    {
        return await context.Likes
             .Select(x => new MemberLikeDto
             {
                 SourceMemberId = x.SourceMemberId,
                 TargetMemberId = x.TargetMemberId
             })
             .FirstOrDefaultAsync(x => x.SourceMemberId == sourceMemberId && x.TargetMemberId == targetMemberId);
    }

    public async Task<PaginatedResult<MemberDto>> GetMemberLikes(LikesParam likesParam)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> likeDtos;

        switch (likesParam.Predicate)
        {
            case "liked":
                likeDtos = likes
                  .Where(x => x.SourceMemberId == likesParam.MemberId)
                  .Select(x => new MemberDto
                  {
                      Id = x.TargetMember.Id,
                      DateOfBirth = x.TargetMember.DateOfBirth,
                      ImageUrl = x.TargetMember.ImageUrl,
                      DisplayName = x.TargetMember.DisplayName,
                      Created = x.TargetMember.Created,
                      LastActive = x.TargetMember.LastActive,
                      Gender = x.TargetMember.Gender,
                      Description = x.TargetMember.Description,
                      City = x.TargetMember.City,
                      Country = x.TargetMember.Country,
                  });
                break;
            case "likedBy":
                likeDtos = likes
                    .Where(x => x.TargetMemberId == likesParam.MemberId)
                    .Select(x => new MemberDto
                    {
                        Id = x.SourceMember.Id,
                        DateOfBirth = x.SourceMember.DateOfBirth,
                        ImageUrl = x.SourceMember.ImageUrl,
                        DisplayName = x.SourceMember.DisplayName,
                        Created = x.SourceMember.Created,
                        LastActive = x.SourceMember.LastActive,
                        Gender = x.SourceMember.Gender,
                        Description = x.SourceMember.Description,
                        City = x.SourceMember.City,
                        Country = x.SourceMember.Country,
                    });
                break;
            default:
                var likeIds = await GetCurrentMemberLikeIds(likesParam.MemberId);
                likeDtos = likes
                    .Where(x => x.TargetMemberId == likesParam.MemberId && likeIds.Contains(x.SourceMemberId))
                    .Select(x => new MemberDto
                    {
                        Id = x.SourceMember.Id,
                        DateOfBirth = x.SourceMember.DateOfBirth,
                        ImageUrl = x.SourceMember.ImageUrl,
                        DisplayName = x.SourceMember.DisplayName,
                        Created = x.SourceMember.Created,
                        LastActive = x.SourceMember.LastActive,
                        Gender = x.SourceMember.Gender,
                        Description = x.SourceMember.Description,
                        City = x.SourceMember.City,
                        Country = x.SourceMember.Country,
                    });
                break;
        }

        return await PaginationHelper.CreateAsync(likeDtos, likesParam.PageNumber, likesParam.PageSize);
    }
}
