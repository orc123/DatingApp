using API.DTOs;
using API.Entities;

namespace API.Repositories;

public interface IMemberRepository
{
    void Update(Member member);
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<MemberDto>> GetMembersAsync();
    Task<MemberDto?> GetMemberByIdAsync(string id);
    Task<IReadOnlyList<PhotoDto>> GetPhotosForMemberAsync(string memberId);
}
