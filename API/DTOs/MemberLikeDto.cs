namespace API.DTOs;

public class MemberLikeDto
{
    public required string SourceMemberId { get; set; }
    public MemberDto SourceMember { get; set; } = null!;
    public required string TargetMemberId { get; set; }
    public MemberDto TargetMember { get; set; } = null!;
}
