using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Utils;
using InfoResponse = ProLinked.Application.Contracts.Identity.DTOs.InfoResponse;

namespace ProLinked.Application.Services.Identity;

public class UserService: IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IBlobManager _blobManager;
    private readonly EmailAddressAttribute _emailAddressAttribute = new();
    private readonly PhoneAttribute _phoneAttribute = new();

    public UserService(
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore, IBlobManager blobManager)
    {
        _userManager = userManager;
        _userStore = userStore;
        _blobManager = blobManager;
    }

    public async Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }

    public async Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        Guid userId)
    {
        if (await _userManager.FindByIdAsync(userId.ToString()) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await ResponseGenerator.CreateInfoResponseAsync(user, _userManager));
    }

    public async Task<Results<Ok<InfoResponse[]>, NotFound>> FindAsync(
        string name)
    {
        var tasks = _userManager.
            Users.
            Where(e => (e.Name+e.Surname).Contains(name, StringComparison.CurrentCultureIgnoreCase)).
            Select(e => ResponseGenerator.CreateInfoResponseAsync(e, _userManager)).
            ToList();

        var result = await Task.WhenAll(tasks);
        return TypedResults.Ok(result);
    }

    public async Task<Results<NoContent, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        UpdateIdentityDto input,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (!input.NewEmail.IsNullOrWhiteSpace() && !_emailAddressAttribute.IsValid(input.NewEmail))
        {
            return ResponseGenerator.CreateValidationProblem(
                IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(input.NewEmail)));
        }

        if (!input.NewPhoneNumber.IsNullOrWhiteSpace())
        {
            if (!_phoneAttribute.IsValid(input.NewPhoneNumber))
            {
                return ResponseGenerator.CreateValidationProblem(
                    IdentityResult.Failed(_userManager.ErrorDescriber.InvalidPhone(input.NewPhoneNumber)));
            }
            await _userStore.SetCompanyAsync(user, input.NewCompany, cancellationToken);
        }

        if (!input.NewName.IsNullOrWhiteSpace())
        {
            await _userStore.SetNameAsync(user, input.NewName, cancellationToken);
        }

        if (!input.NewSurname.IsNullOrWhiteSpace())
        {
            await _userStore.SetSurnameAsync(user, input.NewSurname, cancellationToken);
        }

        if (!input.NewSummary.IsNullOrWhiteSpace())
        {
            await _userStore.SetSummaryAsync(user, input.NewSummary, cancellationToken);
        }

        if (!input.NewJobTitle.IsNullOrWhiteSpace())
        {
            await _userStore.SetJobTitleAsync(user, input.NewJobTitle, cancellationToken);
        }

        if (!input.NewCompany.IsNullOrWhiteSpace())
        {
            await _userStore.SetCompanyAsync(user, input.NewCompany, cancellationToken);
        }

        if (!input.NewCity.IsNullOrWhiteSpace())
        {
            await _userStore.SetCityAsync(user, input.NewCity, cancellationToken);
        }

        if (input.NewDateOfBirth is not null)
        {
            await _userStore.SetDateOfBirthAsync(user, input.NewDateOfBirth, cancellationToken);
        }

        if (!string.IsNullOrEmpty(input.NewPassword))
        {
            if (string.IsNullOrEmpty(input.OldPassword))
            {
                return ResponseGenerator.CreateValidationProblem("OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
            }

            var changePasswordResult =
                await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return ResponseGenerator.CreateValidationProblem(changePasswordResult);
            }
        }

        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, NotFound>> UpdatePhotographAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile photograph,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (user.PhotographId is not null)
        {
            var oldBlob = await _blobManager.GetAsync(user.PhotographId.Value, cancellationToken);
            await _blobManager.DeleteAsync(oldBlob.Info, cancellationToken);
        }

        var data = await photograph.OpenReadStream().GetAllBytesAsync(cancellationToken);
        var updatedBlob = await _blobManager.SaveAsync(
            user.Id,
            photograph.FileName,
            data,
            cancellationToken);

        await _userStore.SetPhotographIdAsync(user, updatedBlob.Id, cancellationToken);
        await _userManager.UpdateAsync(user);
        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, NotFound>> UpdateCurriculumVitaeAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile curriculumVitae,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (user.CurriculumVitaeId is not null)
        {
            var oldBlob = await _blobManager.GetAsync(user.CurriculumVitaeId.Value, cancellationToken);
            await _blobManager.DeleteAsync(oldBlob.Info, cancellationToken);
        }

        var data = await curriculumVitae.OpenReadStream().GetAllBytesAsync(cancellationToken);
        var updatedBlob = await _blobManager.SaveAsync(
            user.Id,
            curriculumVitae.FileName,
            data,
            cancellationToken);

        await _userStore.SetCurriculumVitaeIdAsync(user, updatedBlob.Id, cancellationToken);
        await _userManager.UpdateAsync(user);

        return TypedResults.NoContent();
    }
}