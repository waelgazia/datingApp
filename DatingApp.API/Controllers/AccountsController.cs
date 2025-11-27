using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using DatingApp.API.Base;
using DatingApp.API.DTOs;
using DatingApp.API.Mapping;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

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

            return Ok(await newUser.ToUserDto(_tokenService));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(AccountLoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password!");
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return Unauthorized("Invalid email or password!");
            }

            return Ok(await user.ToUserDto(_tokenService));
        }
    }
}