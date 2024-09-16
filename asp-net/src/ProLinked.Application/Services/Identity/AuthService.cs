using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Extensions;
using ProLinked.Infrastructure.Identity.Auth;
using ProLinked.Infrastructure.Identity.DTOs;
using LoginRequest = ProLinked.Infrastructure.Identity.DTOs.LoginRequest;
using RefreshRequest = ProLinked.Infrastructure.Identity.DTOs.RefreshRequest;
using RegisterRequest = ProLinked.Infrastructure.Identity.DTOs.RegisterRequest;

namespace ProLinked.Application.Services.Identity;

public class AuthService: IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly EmailAddressAttribute _emailAddressAttribute = new();
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IStringLocalizer _l;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IUserStore<AppUser> userStore,
        IJwtTokenService jwtTokenService,
        IStringLocalizer l)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userStore = userStore;
        _jwtTokenService = jwtTokenService;
        _l = l;
    }

    public async Task<Results<Ok<RegistrationResponse>, ValidationProblem>> RegisterAsync(
        RegisterRequest input,
        CancellationToken cancellationToken = default)
    {
        var emailStore = (IUserEmailStore<AppUser>)_userStore;
        var email = input.Email;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return ResponseGenerator.CreateValidationProblem(IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new AppUser();
        await _userStore.SetUserNameAsync(user, input.UserName, cancellationToken);
        await _userStore.SetNameAsync(user, input.Name, cancellationToken);
        await _userStore.SetSurnameAsync(user, input.Surname, cancellationToken);
        await _userStore.SetDateOfBirthAsync(user, input.DateOfBirth, cancellationToken);

        await emailStore.SetEmailAsync(user, email, cancellationToken);
        var result = await _userManager.CreateAsync(user, input.Password);

        if (!result.Succeeded)
        {
            return ResponseGenerator.CreateValidationProblem(result);
        }

        return TypedResults.Ok(new RegistrationResponse()
        {
            Email = email,
            Username = input.UserName,
        });
    }

    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> LoginAsync(
        LoginRequest login)
    {
        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;

        var user = await _userManager.FindByNameAsync(login.Username);
        if (user is null)
        {
            return TypedResults.Problem(_l["LoginError:UserNotFound"], statusCode: StatusCodes.Status401Unauthorized);
        }
        var result =
            await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        var token = await _jwtTokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _jwtTokenService.GenerateRefreshToken();

        var expirationDate = (long)TimeSpan.FromHours(AuthSettings.PrivateKeyExpirationInHours).TotalSeconds;

        await _userStore.SetRefreshTokenAsync(user, refreshToken);
        await _userStore.SetRefreshTokenExpirationAsync(user, DateTime.UtcNow.AddHours(AuthSettings.RefreshKeyExpirationInHours));
        await _userManager.UpdateAsync(user);

        return TypedResults.Ok(
            new AccessTokenResponse
            {
                AccessToken = token,
                ExpiresIn = expirationDate,
                RefreshToken = refreshToken
            }
        );
    }

    public async Task<Results<Ok, ProblemHttpResult>> LogoutAsync()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> RefreshAsync(
            RefreshRequest refreshRequest)
    {

        var user = await _userManager.FindByIdAsync(refreshRequest.UserId.ToString());
        if (user is null)
        {
            return TypedResults.Problem(_l["Error:UserNotFound"], statusCode: StatusCodes.Status401Unauthorized);
        }

        if (user.RefreshToken != refreshRequest.RefreshToken)
        {
            return TypedResults.Problem(_l["Error:InvalidToken"], statusCode: StatusCodes.Status401Unauthorized);
        }

        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
        var refreshToken= await _jwtTokenService.GenerateRefreshToken();
        var expirationDate = (long)TimeSpan.FromHours(AuthSettings.PrivateKeyExpirationInHours).TotalSeconds;

        await _userStore.SetRefreshTokenAsync(user, refreshToken);
        await _userStore.SetRefreshTokenExpirationAsync(user, DateTime.UtcNow.AddHours(AuthSettings.RefreshKeyExpirationInHours));
        await _userManager.UpdateAsync(user);

        return TypedResults.Ok(new AccessTokenResponse()
        {
            AccessToken = accessToken,
            ExpiresIn = expirationDate,
            RefreshToken = refreshToken
        });


    }
}