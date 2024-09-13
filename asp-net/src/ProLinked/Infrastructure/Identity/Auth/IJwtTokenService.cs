using ProLinked.Domain.Identity;

namespace ProLinked.Infrastructure.Identity.Auth;

public interface IJwtTokenService
{
    Task<string> GenerateRefreshToken();
    Task<string> GenerateAccessTokenAsync(AppUser user);
}
