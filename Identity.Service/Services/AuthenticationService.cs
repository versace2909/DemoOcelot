using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Service.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Service.Services;

public class AuthenticationService : IAuthenticationService
{
    public string GenerateJwtToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisIsMyVerySecretKey@12345##$%^");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("id", userId),
                    new Claim("scope", "abc"),
                    new Claim("scope", "abce")
                }),

            Audience = "weatherforecast",
            Issuer = "https://localhost:7108",
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}