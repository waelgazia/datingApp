using System.Text;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using DatingApp.API.Base;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using DatingApp.API.Extensions;

namespace DatingApp.API.Controllers
{
    public class AccountsController(AppDbContext _dbContext, ITokenService _tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>>
            Register([FromBody] AccountForCreationDto accountDto)
        {
            if (await EmailExists(accountDto.Email))
            {
                return BadRequest("The provided email is already registered!");
            }

            using HMACSHA512 hmac = new HMACSHA512();

            AppUser newUser = new AppUser
            {
                DisplayName = accountDto.DisplayName,
                Email = accountDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(accountDto.Password)),
                PasswordSalt = hmac.Key
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return Ok(newUser.ToDto(_tokenService));
        }

        private async Task<bool> EmailExists(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(AccountLoginDto loginDto)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid email address!");
            }

            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash))
                return Unauthorized("Invalid password!");

            return Ok(user.ToDto(_tokenService));
        }
    }
}