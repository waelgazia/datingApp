using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs;

public class AccountForCreationDto
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(4)]
    [MaxLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }
}
