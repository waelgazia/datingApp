using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Services;

public class TokenService(IConfiguration _configuration, UserManager<AppUser> _userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        string tokenKey = _configuration["Authentication:TokenSecretKey"]
            ?? throw new NullReferenceException("Can not get token secret key!");
        if (tokenKey.Length < 64)
        {
            throw new InvalidOperationException("Your token secret key needs to be >= 64 characters");
        }

        // Set the key for encrypting the JWT signature, and the signing algorithm
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        List<Claim> claims = new List<Claim>
        {
            new (ClaimTypes.Email, user.Email!),
            new (ClaimTypes.NameIdentifier, user.Id),
        };

        // add the user roles to the token
        IList<string> roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = signingCredentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}