using System.Security.Claims;

namespace CRM_Backend.Auth;

public interface IJwtService
{
    string GenerateSecurityToken(string id, string email, IEnumerable<string> roles, IEnumerable<Claim> userClaims);
}
