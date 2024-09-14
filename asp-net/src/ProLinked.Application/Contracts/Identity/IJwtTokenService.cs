using ProLinked.Domain.Entities.Identity;

namespace ProLinked.Application.Contracts.Identity;

public interface IJwtTokenService
{
    Task<string> GenerateRefreshToken();
    Task<string> GenerateAccessTokenAsync(AppUser user);
}
