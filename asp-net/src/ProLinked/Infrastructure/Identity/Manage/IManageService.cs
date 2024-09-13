using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using InfoResponse = ProLinked.Infrastructure.Identity.DTOs.InfoResponse;

namespace ProLinked.Infrastructure.Identity.Manage;

public interface IManageService
{
    Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal);

    Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        InfoRequest infoRequest);
}
