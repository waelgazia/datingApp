using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Entities;

public class AppUser : IdentityUser
{
    public required string DisplayName { get; set; }
    public string? ImageUrl { get; set; }
    public string? RefreshedToken { get; set; }
    public DateTime? RefreshedTokenExpiry { get; set; }

    // navigation property
    public Member Member { get; set; } = null!;
}
