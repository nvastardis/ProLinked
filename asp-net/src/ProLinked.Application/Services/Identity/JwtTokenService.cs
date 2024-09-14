using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Infrastructure.Identity.Auth;

namespace ProLinked.Application.Services.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly UserManager<AppUser> _userManager;

    public JwtTokenService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerateRefreshToken()
    {
        return await Task.FromResult(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));
    }

    public async Task<string> GenerateAccessTokenAsync(AppUser user)
    {
        return await GenerateTokenAsync(user, AuthSettings.PrivateKey, AuthSettings.PrivateKeyExpirationInHours);
    }


    private async Task<string> GenerateTokenAsync(AppUser user, string secret, int hours)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = await GenerateClaimsAsync(user),
            Expires = DateTime.UtcNow.AddHours(hours),
            SigningCredentials = credentials,
            Issuer = AuthSettings.Issuer,
            Audience = AuthSettings.Audience
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
