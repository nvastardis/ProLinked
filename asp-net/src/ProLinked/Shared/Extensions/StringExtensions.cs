﻿namespace ProLinked.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(
        this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}
