using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginDto
{
    [EmailAddress]
    public string Email { get; set; } = "";
    [Required]
    [MinLength(4)]
    public string Password { get; set; } = "";
}
