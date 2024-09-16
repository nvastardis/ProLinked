using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Domain.Entities.Identity;
using InfoResponse = ProLinked.Application.DTOs.Identity.InfoResponse;

namespace ProLinked.Application.Services.Identity;

public class ManageService: IManageService
{
    private UserManager<AppUser> _userManager;
    private readonly EmailAddressAttribute _emailAddressAttribute = new();

    public ManageService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }

    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        InfoRequest infoRequest)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
        {
            return ResponseGenerator.CreateValidationProblem(
                IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
        }

        if (!string.IsNullOrEmpty(infoRequest.NewPassword))
        {
            if (string.IsNullOrEmpty(infoRequest.OldPassword))
            {
                return ResponseGenerator.CreateValidationProblem("OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
            }

            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return ResponseGenerator.CreateValidationProblem(changePasswordResult);
            }
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }


}
