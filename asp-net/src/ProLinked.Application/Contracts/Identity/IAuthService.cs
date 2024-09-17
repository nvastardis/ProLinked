using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using ProLinked.Application.DTOs.Identity;
using System.Security.Claims;

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
        ClaimsPrincipal claimsPrincipal,
        RefreshRequest refreshRequest);
}