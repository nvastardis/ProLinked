using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Infrastructure.Identity.Manage;
using InfoResponse = ProLinked.Infrastructure.Identity.DTOs.InfoResponse;

namespace ProLinked.Controllers.Identity;

[ApiController]
[Route("identity/manage")]
public class ManageController: Controller
{
    private readonly ManageService _manageService;

    public ManageController(ManageService manageService)
    {
        _manageService = manageService;
    }

    [HttpGet]
    [Route("info")]
    [AllowAnonymous]
    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> Info()
    {
        return await _manageService.InfoAsync(User);
    }

    [HttpPost]
    [Route("update")]
    [AllowAnonymous]
    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> Update(
        [FromBody] InfoRequest input)
    {
        return await _manageService.UpdateAsync(User, input);
    }
}
