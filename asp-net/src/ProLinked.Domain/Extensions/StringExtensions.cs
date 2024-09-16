﻿using ProLinked.Domain.Shared.Utils;
using System.Text;

namespace ProLinked.Domain.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(
        this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string Left(this string str, int len)
    {
        Check.NotNull(str, nameof(str));

        if (str.Length < len)
        {
            throw new ArgumentException("len argument can not be bigger than given string's length!");
        }

        return str.Substring(0, len);
    }
}