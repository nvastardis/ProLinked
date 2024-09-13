using Microsoft.EntityFrameworkCore;
using ProLinked.Application.Services.Resumes;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Repositories.Resumes;

public class ResumeRepository: ProLinkedBaseRepository<Resume,Guid>, IResumeRepository
{
    public ResumeRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Resume.UserId)} DESC";
    }

    public async Task<List<EducationStep>> GetListEducationStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await WithDetailsAsync(cancellationToken);
        var query = from resume in queryable
                    where resume.UserId == userId
                    select resume.Education.ToList();

        return await query.FirstAsync(cancellationToken);
    }

    public async Task<List<ExperienceStep>> GetListExperienceStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await WithDetailsAsync(cancellationToken);
        var query = from resume in queryable
            where resume.UserId == userId
            select resume.Experience.ToList();

        return await query.FirstAsync(cancellationToken);
    }

    public async Task<List<ResumeSkill>> GetListResumeSkillByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await WithDetailsAsync(cancellationToken);
        var query = from resume in queryable
            where resume.UserId == userId
            select resume.ResumeSkills.ToList();

        return await query.FirstAsync(cancellationToken);
    }

    public override async Task<IQueryable<Resume>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Include(e => e.Education)
            .ThenInclude(e => e.RelatedSkills)
            .Include(e => e.Experience)
            .ThenInclude(e => e.RelatedSkills)
            .Include(e => e.ResumeSkills);
    }
}
