using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Identity;
using ProLinked.Domain.Shared.Utils;
using System.Security.Claims;
using LoginRequest = ProLinked.Application.Contracts.Identity.DTOs.LoginRequest;
using RefreshRequest = ProLinked.Application.Contracts.Identity.DTOs.RefreshRequest;
using RegisterRequest = ProLinked.Application.Contracts.Identity.DTOs.RegisterRequest;

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
        var phoneNumberStore = (IUserPhoneNumberStore<AppUser>)_userStore;
        var email = input.Email;
        var phoneNumber = input.PhoneNumber;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return ResponseGenerator.CreateValidationProblem(IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new AppUser();
        await emailStore.SetEmailAsync(user, email, cancellationToken);
        await _userStore.SetUserNameAsync(user, input.UserName, cancellationToken);
        await _userStore.SetNameAsync(user, input.Name, cancellationToken);
        await _userStore.SetSurnameAsync(user, input.Surname, cancellationToken);
        await _userStore.SetDateOfBirthAsync(user, input.DateOfBirth, cancellationToken);

        if (!input.Summary.IsNullOrWhiteSpace())
        {
            await _userStore.SetSummaryAsync(user, input.Summary, cancellationToken);
        }
        if (!input.JobTitle.IsNullOrWhiteSpace())
        {
            await _userStore.SetJobTitleAsync(user, input.JobTitle, cancellationToken);
        }
        if (!input.Company.IsNullOrWhiteSpace())
        {
            await _userStore.SetCompanyAsync(user, input.Company, cancellationToken);
        }

        if (!input.City.IsNullOrWhiteSpace())
        {
            await _userStore.SetCityAsync(user, input.City, cancellationToken);
        }

        if (!phoneNumber.IsNullOrWhiteSpace())
        {
            await phoneNumberStore.SetPhoneNumberAsync(user, phoneNumber, cancellationToken);
        }

        var createResult = await _userManager.CreateAsync(user, input.Password);
        if (!createResult.Succeeded)
        {
            return ResponseGenerator.CreateValidationProblem(createResult);
        }

        var roleAssignmentResult = await _userManager.AddToRoleAsync(user, RoleConsts.UserRoleName);
        if (!roleAssignmentResult.Succeeded)
        {
            return ResponseGenerator.CreateValidationProblem(createResult);
        }

        return TypedResults.Ok(
            new RegistrationResponse
            {
                Email = email,
                Username = input.UserName,
            }
        );
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

        var token = await _jwtTokenService.GenerateAccessTokenAsync(new AppUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname
        });
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
        ClaimsPrincipal claimsPrincipal,
        RefreshRequest refreshRequest)
    {

        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user is null)
        {
            return TypedResults.Problem(_l["Error:UserNotFound"], statusCode: StatusCodes.Status401Unauthorized);
        }

        if (user.RefreshToken != refreshRequest.RefreshToken)
        {
            return TypedResults.Problem(_l["Error:InvalidToken"], statusCode: StatusCodes.Status401Unauthorized);
        }

        var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(new AppUserDto
        {
            Email = user.Email,
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname
        });

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