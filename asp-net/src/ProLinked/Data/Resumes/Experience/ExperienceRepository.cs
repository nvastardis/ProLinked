using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Resumes.Experience;

namespace ProLinked.Data.Resumes.Experience;

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
