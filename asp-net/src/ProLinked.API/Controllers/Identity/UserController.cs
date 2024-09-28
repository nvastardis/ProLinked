using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using System.ComponentModel.DataAnnotations;
using InfoResponse = ProLinked.Application.Contracts.Identity.DTOs.InfoResponse;

namespace ProLinked.API.Controllers.Identity;

[ApiController]
[Route("api/user")]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("info")]
    [Authorize]
    public async Task<Results<Ok<InfoResponse>, NotFound>> Info()
    {
        return await _userService.InfoAsync(User);
    }

    [HttpGet]
    [Route("info/{id}")]
    [Authorize]
    public async Task<Results<Ok<InfoResponse>, NotFound>> Info(
        Guid id)
    {
        return await _userService.InfoAsync(id);
    }


    [HttpGet]
    [Route("find")]
    [Authorize]
    public async Task<Results<Ok<InfoResponse[]>, NotFound>> Info(
        [FromQuery, Required] string name)
    {
        return await _userService.FindAsync(name);
    }


    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<Results<NoContent, ValidationProblem, NotFound>> Update(
        UpdateIdentityDto input)
    {
        return await _userService.UpdateAsync(User, input);
    }

    [HttpPost]
    [Route("update-photograph")]
    [Authorize]
    public async Task<Results<NoContent, NotFound>> UpdatePhotograph(
        IFormFile photograph)
    {
        return await _userService.UpdatePhotographAsync(User, photograph);
    }

    [HttpPost]
    [Route("update-cv")]
    [Authorize]
    public async Task<Results<NoContent, NotFound>> Update(
        IFormFile cv)
    {
        return await _userService.UpdatePhotographAsync(User, cv);
    }
}