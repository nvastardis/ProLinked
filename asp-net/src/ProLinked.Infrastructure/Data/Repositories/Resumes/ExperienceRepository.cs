using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Resumes;

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