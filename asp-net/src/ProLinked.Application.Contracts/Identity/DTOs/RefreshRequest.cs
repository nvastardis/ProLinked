﻿namespace ProLinked.Application.Contracts.Identity.DTOs;

public class RefreshRequest
{
    public required string RefreshToken { get; init; }
}