using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Identity;
using InfoResponse = ProLinked.Application.DTOs.Identity.InfoResponse;

namespace ProLinked.API.Controllers.Identity;

[ApiController]
[Route("api/identity/manage")]
public class ManageController: ControllerBase
{
    private readonly IManageService _manageService;

    public ManageController(IManageService manageService)
    {
        _manageService = manageService;
    }

    [HttpGet]
    [Route("info")]
    [Authorize]
    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> Info()
    {
        return await _manageService.InfoAsync(User);
    }

    [HttpPost]
    [Route("update")]
    [Authorize]
    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> Update(
        InfoRequest input)
    {
        return await _manageService.UpdateAsync(User, input);
    }
}