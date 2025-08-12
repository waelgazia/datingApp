using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs;

public class AccountForCreationDto
{
    [Required]
    public string DisplayName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(4)]
    public string Password { get; set; } = "";
}
