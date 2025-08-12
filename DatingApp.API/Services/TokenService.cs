using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using DatingApp.API.Entities;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Services;

public class TokenService(IConfiguration _configuration) : ITokenService
{
    public string CreateToken(AppUser user)
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
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.NameIdentifier, user.Id),
        };

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