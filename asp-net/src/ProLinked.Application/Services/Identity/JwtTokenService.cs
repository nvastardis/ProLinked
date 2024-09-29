﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using ProLinked.Domain.Entities.Identity;

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
        return await Task.FromResult(
            Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
                )
            );
    }

    public async Task<string> GenerateAccessTokenAsync(
        AppUserDto user)
    {
        return await GenerateTokenAsync(
            user,
            AuthSettings.PrivateKey,
            AuthSettings.PrivateKeyExpirationInHours
        );
    }


    private async Task<string> GenerateTokenAsync(
        AppUserDto user,
        string secret,
        int hours)
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

    private async Task<ClaimsIdentity> GenerateClaimsAsync(
        AppUserDto user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        claims.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));

        var userCore = await _userManager.FindByIdAsync(user.Id.ToString());
        var roles = await _userManager.GetRolesAsync(userCore!);
        foreach (var role in roles)
        {
            if (claims.HasClaim(ClaimTypes.Role, role))
            {
                continue;
            }

            claims.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}