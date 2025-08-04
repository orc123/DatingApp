using API.DTOs;
using API.Entities;
using API.Services;

namespace API.Extensions;

public static class AppUserExtention
{
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = tokenService.CreateToken(user),
            ImageUrl = user.ImageUrl,
        };
    }
}
