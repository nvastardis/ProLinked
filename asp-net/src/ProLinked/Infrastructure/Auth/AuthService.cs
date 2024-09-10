using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Domain.Identity;

namespace ProLinked.Infrastructure.Auth;

public class AuthService
{
    private UserManager<AppUser> _userManager;
    public AuthService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(AppUser user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = await GenerateClaimsAsync(user),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Email ?? string.Empty));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.AddClaim(new Claim(ClaimTypes.Role, role));

        return claims;
    }
}
