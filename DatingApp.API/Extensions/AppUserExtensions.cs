using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Extensions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser appUser, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = appUser.Id,
            Email = appUser.Email,
            DisplayName = appUser.DisplayName,
            Token = tokenService.CreateToken(appUser)
        };
    }
}
