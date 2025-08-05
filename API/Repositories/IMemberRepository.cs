using API.DTOs;
using API.Entities;

namespace API.Repositories;

public interface IMemberRepository
{
    Task<bool> UpdateAsync(string memberId, MemberUpdateDto memberUpdateDto);
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<MemberDto>> GetMembersAsync();
    Task<MemberDto?> GetMemberByIdAsync(string id);
    Task<IReadOnlyList<PhotoDto>> GetPhotosForMemberAsync(string memberId);

    Task<MemberDto?> GetMemberForUpdateAsync(string memberId);
}
