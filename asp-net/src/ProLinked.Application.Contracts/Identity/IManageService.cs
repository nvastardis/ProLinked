using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;
using InfoResponse = ProLinked.Application.Contracts.Identity.DTOs.InfoResponse;

namespace ProLinked.Application.Contracts.Identity;

public interface IManageService
{
    Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal);

    Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        InfoRequest infoRequest);
}