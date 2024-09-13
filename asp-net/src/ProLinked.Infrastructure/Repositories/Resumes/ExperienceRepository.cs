using Microsoft.EntityFrameworkCore;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Repositories.Resumes;

public class ExperienceRepository: ProLinkedBaseRepository<ExperienceStep, Guid>, IExperienceRepository
{
    public ExperienceRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(ExperienceStep.StartDate)} DESC";
    }

    public override async Task<IQueryable<ExperienceStep>> WithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Include(e => e.RelatedSkills);
    }
}
