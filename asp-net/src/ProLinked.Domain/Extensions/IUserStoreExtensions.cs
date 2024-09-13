using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Extensions;

public static class UserStoreExtensions
{
    public static Task SetNameAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        string? name,
        CancellationToken cancellationToken = default (CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof (user));
        Check.NotNullOrWhiteSpace(name, nameof (name));
        user.Name = name!;
        return Task.CompletedTask;
    }

    public static Task<string> GetNameAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.Name);
    }

    public static Task SetSurnameAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        string? surname,
        CancellationToken cancellationToken = default (CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof (user));
        Check.NotNullOrWhiteSpace(surname, nameof (surname));
        user.Surname = surname!;
        return Task.CompletedTask;
    }

    public static Task<string> GetSurnameAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.Surname);
    }

    public static Task SetDateOfBirthAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        DateTime? dateOfBirth,
        CancellationToken cancellationToken = default (CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof (user));
        Check.NotNull(dateOfBirth, nameof (dateOfBirth));
        user.DateOfBirth = dateOfBirth;
        return Task.CompletedTask;
    }

    public static Task<DateTime?> GetDateOfBirthAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.DateOfBirth);
    }

    public static Task SetRefreshTokenAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        string? token,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof (user));
        Check.NotNullOrWhiteSpace(token, nameof (token));
        user.RefreshToken = token;
        return Task.CompletedTask;
    }

    public static Task<string?> GetRefreshTokenAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.RefreshToken);
    }

    public static Task SetRefreshTokenExpirationAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        DateTime? expirationDate,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof (user));
        Check.NotNull(expirationDate, nameof (expirationDate));
        user.RefreshTokenExpirationDate = expirationDate!;
        return Task.CompletedTask;
    }

    public static Task<DateTime?> GetRefreshTokenExpirationAsync(
        this IUserStore<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.RefreshTokenExpirationDate);
    }
}
