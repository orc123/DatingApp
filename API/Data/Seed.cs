using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;
        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        if (memberData == null) return;

        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            Console.WriteLine("No members in seed data.");
            return;
        }

        foreach (var member in members)
        {
            var user = new AppUser
            {
                Id = member.Id,
                Email = member.Email,
                UserName = member.Email,
                DisplayName = member.DisplayName,
                ImageUrl = member.ImageUrl,
                Member = new Member
                {
                    Id = member.Id,
                    DateOfBirth = member.DateOfBirth,
                    ImageUrl = member.ImageUrl,
                    DisplayName = member.DisplayName,
                    Created = member.Created,
                    LastActive = member.LastActive,
                    City = member.City,
                    Country = member.Country,
                    Gender = member.Gender,
                    Description = member.Description
                }
            };

            user.Member.Photos.Add(new Photo
            {
                Url = member.ImageUrl!,
                MemberId = member.Id
            });

            var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            if (!result.Succeeded)
            {
                Console.WriteLine($"Error creating user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                continue;
            }
            await userManager.AddToRoleAsync(user, "Member");
        }

        var adminUser = new AppUser
        {
            Email = "admin@test.com",
            UserName = "admin@test.com",
            DisplayName = "Admin",
        };

        await userManager.CreateAsync(adminUser, "Pa$$w0rd");
        await userManager.AddToRolesAsync(adminUser, ["Admin", "Moderator"]);
    }
}
