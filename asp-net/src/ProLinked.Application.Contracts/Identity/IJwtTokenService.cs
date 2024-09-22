using ProLinked.Application.Contracts.Identity.DTOs;

namespace ProLinked.Application.Contracts.Identity;

public interface IJwtTokenService
{
    Task<string> GenerateRefreshToken();
    Task<string> GenerateAccessTokenAsync(AppUserDto user);
}