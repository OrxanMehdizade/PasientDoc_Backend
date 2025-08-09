using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM_Backend.Auth;

public class JwtService(JwtConfig config) : IJwtService
{
    private readonly JwtConfig _config = config;

    public string GenerateSecurityToken(string id, string name, IEnumerable<string> roles, IEnumerable<Claim> userClaims)
    {
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, name),
            new Claim("userId", id),
            //new Claim(ClaimsIdentity.DefaultRoleClaimType, string.Join(",", roles))
        }.Concat(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));


        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            expires: DateTime.Now.AddMinutes(_config.ExpiresInMinutes),
            signingCredentials: signingCredentials,
            claims: claims
            );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return accessToken;
    }
}