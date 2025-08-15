using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Repositories;

public interface IMemberRepository
{
    Task<bool> UpdateAsync(string memberId, MemberUpdateDto memberUpdateDto);
    Task<PaginatedResult<MemberDto>> GetMembersAsync(MemberParams memberParams);
    Task<MemberDto?> GetMemberByIdAsync(string id);
    Task<IReadOnlyList<PhotoDto>> GetPhotosForMemberAsync(string memberId);

    Task<MemberDto?> GetMemberForUpdateAsync(string memberId);
}
