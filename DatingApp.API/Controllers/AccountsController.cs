using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.DTOs;
using DatingApp.API.Mapping;
using DatingApp.API.Globals;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Extensions;

namespace DatingApp.API.Controllers
{
    public class AccountsController(UserManager<AppUser> _userManager, ITokenService _tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] AccountForCreationDto accountDto)
        {
            AppUser newUser = new AppUser
            {
                DisplayName = accountDto.DisplayName,
                Email = accountDto.Email,
                UserName = accountDto.Email,
                Member = new Member()
                {
                    DisplayName = accountDto.DisplayName,
                    DateOfBirth = accountDto.DateOfBirth,
                    Gender = accountDto.Gender,
                    City = accountDto.City,
                    Country = accountDto.Country
                }
            };
            newUser.Member.Id = newUser.Id;

            var createResult = await _userManager.CreateAsync(newUser, accountDto.Password);
            if (!createResult.Succeeded)
            {
                createResult.Errors.ToList().ForEach(e => ModelState.AddModelError("identity", e.Description));
                return ValidationProblem();
            }
            await _userManager.AddToRoleAsync(newUser, "Member");

            await SetRefreshTokenCookie(newUser);

            return Ok(await newUser.ToUserDto(_tokenService));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(AccountLoginDto loginDto)
        {
            AppUser? user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password!");
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return Unauthorized("Invalid email or password!");
            }

            await SetRefreshTokenCookie(user);

            return Ok(await user.ToUserDto(_tokenService));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            AppUser? user = await _userManager.FindByIdAsync(User.GetMemberId());
            if (user == null) return Unauthorized("Can't perform this action!");

            user.RefreshedToken = null;
            user.RefreshedTokenExpiry = null;
            await _userManager.UpdateAsync(user);

            // remove refresh token cookie
            CookieOptions cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            Response.Cookies.Append(Cookies.RefreshToken, "", cookieOptions);

            return NoContent();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            string? refreshToken = Request.Cookies[Cookies.RefreshToken];

            // This endpoint will be called every 5 minutes from the front-end to refresh the token
            // if the user is still using the app, refreshToken can be null if the user logged out,
            // so we will return a NoContent() instead of Unauthorized() to prevent showing a toast
            // with Unauthorized error in the front-end.
            if (string.IsNullOrWhiteSpace(refreshToken)) return NoContent();

            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(
                u => u.RefreshedToken == refreshToken && u.RefreshedTokenExpiry > DateTime.UtcNow);
            if (user == null) return Unauthorized();

            await SetRefreshTokenCookie(user);

            return Ok(await user.ToUserDto(_tokenService));
        }

        private async Task SetRefreshTokenCookie(AppUser user)
        {
            string refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshedToken = refreshToken;
            user.RefreshedTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            CookieOptions cookieOptions = new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append(Cookies.RefreshToken, refreshToken, cookieOptions);
        }
    }
}