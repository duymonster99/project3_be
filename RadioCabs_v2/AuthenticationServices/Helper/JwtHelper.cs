using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationServices.Helper;

public class JwtHelper
{
    public static string GenerateToken(string identifier, string key, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = Encoding.ASCII.GetBytes(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, identifier),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddSeconds(60),
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(securityKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}