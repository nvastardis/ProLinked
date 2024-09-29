using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using ProLinked.Application.Contracts.Identity.DTOs;
using System.Security.Claims;

namespace ProLinked.Application.Contracts.Identity;

public interface IUserService
{
    Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        ClaimsPrincipal claimsPrincipal);

    Task<Results<Ok<InfoResponse>, NotFound>> InfoAsync(
        Guid userId);

    Task<Results<FileStreamHttpResult, NotFound>> DownloadInfoAsync(
        Guid userId,
        bool inXml = false);

    Task<Results<Ok<InfoResponse[]>, NotFound>> FindAsync(
        string name);

    Task<Results<NoContent, ValidationProblem, NotFound>> UpdateAsync(
        ClaimsPrincipal claimsPrincipal,
        UpdateIdentityDto infoRequest,
        CancellationToken cancellationToken = default);

    Task<Results<NoContent, NotFound>> UpdatePhotographAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile photograph,
        CancellationToken cancellationToken = default);

    Task<Results<NoContent, NotFound>> UpdateCurriculumVitaeAsync(
        ClaimsPrincipal claimsPrincipal,
        IFormFile curriculumVitae,
        CancellationToken cancellationToken = default);
}