using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using ProLinked.Application.DTOs.Identity;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Extensions;

namespace ProLinked.Application.Services.Identity;

public static class ResponseGenerator
{
    public static async Task<InfoResponse> CreateInfoResponseAsync(AppUser user, UserManager<AppUser> userManager)
    {
        return new InfoResponse
        {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            UserName = await userManager.GetUserNameAsync(user) ?? throw new NotSupportedException("User must have username"),
            Name = await userManager.GetNameAsync(user) ?? throw new NotSupportedException("User must have a name"),
            Surname = await userManager.GetSurnameAsync(user) ?? throw new NotSupportedException("User must have a surname"),
            DateOfBirth = await userManager.GetDateOfBirthAsync(user) ?? throw new NotSupportedException("User must have a Date of Birth")
        };
    }

    public static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { errorCode, [errorDescription] }
        });

    public static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
