using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Member = new Member
            {
                City = registerDto.City,
                Country = registerDto.Country,
                DisplayName = registerDto.DisplayName,
                Gender = registerDto.Gender,
                DateOfBirth = registerDto.DateOfBirth
            }
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
            return ValidationProblem(ModelState);
        }

        await userManager.AddToRoleAsync(user, "Member");

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null) return Unauthorized("Invalid email address");
       
        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized("Invalid password");

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refresherToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refresherToken)) return Unauthorized("No refresh token provided");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefresherToken == refresherToken
                        && u.RefresherTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }
        await SetRefreshTokenCookie(user);
        return await user.ToDto(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refresherToken = tokenService.GenerateRefresherToken();
        user.RefresherToken = refresherToken;
        user.RefresherTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = user.RefresherTokenExpiry,
            SameSite = SameSiteMode.Strict,
            Secure = true // Set to true if using HTTPS
        };

        Response.Cookies.Append("refreshToken", refresherToken, cookieOptions);
    }
}
