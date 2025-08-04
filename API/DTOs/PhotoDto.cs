using API.Entities;

namespace API.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; }

    // Navigation properties
    public Member Member { get; set; } = null!;
    public string MemberId { get; set; } = null!;
}
