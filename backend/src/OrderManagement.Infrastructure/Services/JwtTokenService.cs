using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;namespace OrderManagement.Infrastructure.Services;
public sealed class JwtTokenService(IConfiguration config):IJwtTokenService
{
    public string CreateToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken( new JwtSecurityToken(
            issuer:config["Jwt:Issuer"],
            audience:config["Jwt:Audience"],
            claims:[new(ClaimTypes.Email, email)],
            expires:DateTime.UtcNow.AddHours(8),
            signingCredentials:creds)
        );
    }
}
