﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Resumes;

namespace ProLinked.API.Controllers.Resumes;

[ApiController]
[Route("api/skill")]
public class SkillController: ProLinkedController
{
    private ISkillService _skillService;
    public SkillController(
        ILogger logger,
        ISkillService skillService)
        : base(logger)
    {
        _skillService = skillService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<SkillDto>>, ProblemHttpResult>> GetSkillListAsync(
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _skillService.GetSkillListAsync(
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("create")]
    public async Task<Results<NoContent, ProblemHttpResult>> CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        return await NoContentWithStandardExceptionHandling(
            _skillService.CreateSkillAsync(
                title,
                cancellationToken)
        );
    }
}