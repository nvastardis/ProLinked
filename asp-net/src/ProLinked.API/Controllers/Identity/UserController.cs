using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Identity;
using ProLinked.Application.Contracts.Identity.DTOs;
using ProLinked.Domain.Shared.Identity;
using System.ComponentModel.DataAnnotations;
using InfoResponse = ProLinked.Application.Contracts.Identity.DTOs.InfoResponse;

namespace ProLinked.API.Controllers.Identity;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController: ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("info")]
    public async Task<Results<Ok<InfoResponse>, NotFound>> Info()
    {
        return await _userService.InfoAsync(User);
    }

    [HttpGet]
    [Route("info/{id}")]
    public async Task<Results<Ok<InfoResponse>, NotFound>> Info(
        Guid id)
    {
        return await _userService.InfoAsync(id);
    }

    [HttpGet]
    [Route("info/{id}/download")]
    [Authorize(Roles=RoleConsts.AdministrationRoleName)]
    public async Task<Results<FileStreamHttpResult, NotFound>> DownloadInfo(
        Guid id,
        [FromQuery]bool inXml = false)
    {
        return await _userService.DownloadInfoAsync(id, inXml);
    }

    [HttpGet]
    [Route("find")]
    public async Task<Results<Ok<InfoResponse[]>, NotFound>> Info(
        [FromQuery, Required] string name)
    {
        return await _userService.FindAsync(name);
    }


    [HttpPost]
    [Route("update")]
    public async Task<Results<NoContent, ValidationProblem, NotFound>> Update(
        UpdateIdentityDto input)
    {
        return await _userService.UpdateAsync(User, input);
    }

    [HttpPost]
    [Route("update-photograph")]
    public async Task<Results<NoContent, NotFound>> UpdatePhotograph(
        IFormFile photograph)
    {
        return await _userService.UpdatePhotographAsync(User, photograph);
    }

    [HttpPost]
    [Route("update-cv")]
    public async Task<Results<NoContent, NotFound>> Update(
        IFormFile cv)
    {
        return await _userService.UpdatePhotographAsync(User, cv);
    }
}