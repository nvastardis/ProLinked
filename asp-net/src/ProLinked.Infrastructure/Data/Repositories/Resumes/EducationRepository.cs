using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Resumes;

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