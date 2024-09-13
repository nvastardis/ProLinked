using Microsoft.EntityFrameworkCore;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Repositories.Resumes;

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
