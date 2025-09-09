using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

using Microsoft.EntityFrameworkCore;

using DatingApp.API.DTOs;
using DatingApp.API.Entities;

namespace DatingApp.API.Data;

public class Seed
{
    public static async Task SeedUserAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        string memberDate = await File.ReadAllTextAsync("Data/UserSeedData.json");
        List<SeedUserDto>? members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberDate);

        if (members == null)
        {
            Console.WriteLine("No members in seed data");
            return;
        }

        foreach (var member in members)
        {
            using var hmac = new HMACSHA512();

            AppUser user = new AppUser()
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                ImageUrl = member.ImageUrl,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                PasswordSalt = hmac.Key,
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

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}
