using Microsoft.AspNetCore.Identity;

namespace ProLinked.Domain.Extensions;

public static class IdentityErrorDescriberExtensions
{
    public static IdentityError InvalidPhone(this IdentityErrorDescriber item, string? phoneNumber)
    {
        return new IdentityError()
        {
            Code = nameof(InvalidPhone),
            Description = $"Invalid Phone Number {phoneNumber}"
        };
    }
}