﻿using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.DTOs.Identity;

namespace ProLinked.API.Controllers.Identity;

[ApiController]
[Route("api/identity/auth")]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<Results<Ok<RegistrationResponse>, ValidationProblem>> Register(
        [FromBody] RegisterRequest input,
        CancellationToken cancellationToken = default) =>
        await _authService.RegisterAsync(input, cancellationToken);


    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Login(
        [FromBody] LoginRequest input)
    {
        var result = await _authService.LoginAsync(input);
        if (result.Result is Ok<AccessTokenResponse> accessTokenResponse)
        {
            SetRefreshTokenCookie(accessTokenResponse.Value!.RefreshToken);
        }
        return result;
    }


    [HttpPost]
    [Route("logout")]
    [Authorize]
    public async Task<Results<Ok, ProblemHttpResult>> Logout() =>
        await _authService.LogoutAsync();

    [HttpPost]
    [Route("refresh")]
    [Authorize]
    public async Task<Results<Ok<AccessTokenResponse>, ProblemHttpResult>> Refresh(
        [FromBody] RefreshRequest refreshRequest)
    {
        var result = await _authService.RefreshAsync(User, refreshRequest);
        if (result.Result is Ok<AccessTokenResponse> accessTokenResponse)
        {
            SetRefreshTokenCookie(accessTokenResponse.Value!.RefreshToken);
        }
        return result;
    }


    private void SetRefreshTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }

}