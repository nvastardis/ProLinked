﻿using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Extensions;

public static class UserManagerExtensions
{
    public static Task<string> GetNameAsync(
        this UserManager<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.Name);
    }

    public static Task<string> GetSurnameAsync(
        this UserManager<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.Surname);
    }

    public static Task<DateTime?> GetDateOfBirthAsync(
        this UserManager<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.DateOfBirth);
    }

    public static Task<string?> GetRefreshTokenAsync(
        this UserManager<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.RefreshToken);
    }

    public static Task<DateTime?> GetRefreshTokenExpirationDateAsync(
        this UserManager<AppUser> item,
        AppUser user,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.RefreshTokenExpirationDate);
    }
}