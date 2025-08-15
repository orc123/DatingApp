using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Repositories;

public interface ILikeRepository
{
    Task<MemberLikeDto?> GetMemberLike(string sourceMemberId, string targetMemberId);
    Task<PaginatedResult<MemberDto>> GetMemberLikes(LikesParam likesParam);
    Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);

    Task<int> DeleteLikeAsync(MemberLikeDto likeDto);
    Task<int> AddLikeAsync(MemberLikeDto likeDto);
}
