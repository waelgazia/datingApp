using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using DatingApp.API.DTOs;
using DatingApp.API.Globals;
using DatingApp.API.Entities;

namespace DatingApp.API.Data;

public class Seed
{
    public static async Task SeedUserAsync(UserManager<AppUser> userManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        string memberDate = await File.ReadAllTextAsync("Data/UserSeedData.json");
        List<SeedUserDto>? members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberDate);

        if (members == null)
        {
            Console.WriteLine("No members in seed data");
            return;
        }

        foreach (var member in members)
        {
            AppUser user = new AppUser()
            {
                Id = member.Id,
                Email = member.Email,
                UserName = member.Email,
                DisplayName = member.DisplayName,
                ImageUrl = member.ImageUrl,
                Member = new Member
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    Description = member.Description,
                    DateOfBirth = member.DateOfBirth,
                    ImageUrl = member.ImageUrl,
                    Gender = member.Gender,
                    City = member.City,
                    Country = member.Country,
                    LastActive = member.LastActive,
                    Created = member.Created
                }
            };

            user.Member.Photos.Add(new Photo
            {
                Url = member.ImageUrl!,
                MemberId = member.Id
            });

            IdentityResult result = await userManager.CreateAsync(user, "Pa$$w0rd");

            StringBuilder sb = new StringBuilder();
            if (!result.Succeeded)
            {
                result.Errors?.ToList().ForEach(e => sb.AppendLine(e.Description));
                Console.WriteLine($"======================= Error =======================\n{sb}");
            }

            await userManager.AddToRoleAsync(user, Roles.MEMBER);
        }

        AppUser admin = new AppUser
        {
            UserName = "admin@test.com",
            Email = "admin@test.com",
            DisplayName = "Admin"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin, [Roles.ADMIN, Roles.MODERATOR]);
    }
}
