using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser
{
    public required string DisplayName { get; set; }
    public string? ImageUrl { get; set; }

    public string? RefresherToken { get; set; }
    public DateTime? RefresherTokenExpiry { get; set; }
    // Navigation properties
    public Member Member { get; set; } = null!;
}
