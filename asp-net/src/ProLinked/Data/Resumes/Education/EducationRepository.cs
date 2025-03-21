﻿using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Resumes.Education;

namespace ProLinked.Data.Resumes.Education;

public class EducationRepository: ProLinkedBaseRepository<EducationStep, Guid>, IEducationRepository
{
    public EducationRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(EducationStep.StartDate)} DESC";
    }

    public override async Task<IQueryable<EducationStep>> WithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Include(e => e.RelatedSkills);
    }
}
