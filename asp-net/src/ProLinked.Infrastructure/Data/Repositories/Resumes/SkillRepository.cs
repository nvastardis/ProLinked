using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Resumes;

public class SkillRepository: ProLinkedBaseRepository<Skill, Guid>, ISkillRepository
{
    public SkillRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Skill.Title)} ASC";
    }


    public async Task<Skill?> FindByTitleAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        var result = await queryable.
            FirstOrDefaultAsync(
                e => e.Title.Trim().ToLower() == title.Trim().ToLower(),
                cancellationToken);

        return result;
    }
}