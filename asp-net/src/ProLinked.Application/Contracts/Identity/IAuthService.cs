using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using ProLinked.Infrastructure.Identity.DTOs;

namespace ProLinked.Application.Contracts.Identity;

public interface IAuthService
{
    Task<Results<Ok<RegistrationResponse>, ValidationProblem>> RegisterAsync(
        RegisterRequest input,
        CancellationToken cancellationToken = default);

    Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> LoginAsync(
        LoginRequest login);

    Task<Results<Ok, ProblemHttpResult>> LogoutAsync();

    Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> RefreshAsync(
        RefreshRequest refreshRequest);
}
