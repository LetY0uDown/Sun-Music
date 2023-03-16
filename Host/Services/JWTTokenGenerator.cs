using Host.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Host.Services;

public class JWTTokenGenerator : IAuthTokenGen
{
    private readonly IConfiguration _config;

    private readonly SymmetricSecurityKey _securityKey;

    public JWTTokenGenerator (IConfiguration config)
    {
        _config = config;

        _securityKey = new(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
    }

    public string GetToken (string username, string password)
    {
        List<Claim> claims = new() {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.UserData, password)
        };

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: new(_securityKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}