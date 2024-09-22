using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.DTOs.Resumes;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Infrastructure.Data.Repositories.Resumes;

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

        return await query.SingleAsync(cancellationToken);
    }

    public async Task<List<EducationStep>> GetListEducationStepAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var resume = await GetAsync(resumeId, includeDetails: true, cancellationToken);
        return resume.Education.ToList();
    }

    public async Task<List<ExperienceStep>> GetListExperienceStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await WithDetailsAsync(cancellationToken);
        var query = from resume in queryable
            where resume.UserId == userId
            select resume.Experience.ToList();

        return await query.SingleAsync(cancellationToken);
    }

    public async Task<List<ExperienceStep>> GetListExperienceStepAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var resume = await GetAsync(resumeId, includeDetails: true, cancellationToken);
        return resume.Experience.ToList();
    }

    public async Task<List<Skill>> GetListResumeSkillByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        var resumeSkillQueryable = (await GetDbContextAsync(cancellationToken)).Set<ResumeSkill>().AsQueryable();
        var skillQueryable = (await GetDbContextAsync(cancellationToken)).Set<Skill>().AsQueryable();

        var query =
            from resume in queryable
            join resumeSkill in resumeSkillQueryable on resume.Id equals resumeSkill.ResumeId
            join skill in skillQueryable on resumeSkill.SkillId equals skill.Id
            where resume.UserId == userId
            select skill;

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Skill>> GetListResumeSkillAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        var resumeSkillQueryable = (await GetDbContextAsync(cancellationToken)).Set<ResumeSkill>().AsQueryable();
        var skillQueryable = (await GetDbContextAsync(cancellationToken)).Set<Skill>().AsQueryable();

        var query =
            from resume in queryable
            join resumeSkill in resumeSkillQueryable on resume.Id equals resumeSkill.ResumeId
            join skill in skillQueryable on resumeSkill.SkillId equals skill.Id
            where resume.Id == resumeId
            select skill;

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ResumeWithDetails> GetWithDetailsAsync(Guid resumeId, CancellationToken cancellationToken)
    {
        var queryable = await GetQueryableAsync(cancellationToken);
        var userQueryable = (await GetDbContextAsync(cancellationToken)).Set<AppUser>().AsQueryable();

        var resumeQuery =
            from resume in queryable
            join user in userQueryable on resume.UserId equals user.Id
            where resume.Id == resumeId
            select new ResumeWithDetails
            {
                ResumeId = resume.Id,
                UserId = resume.UserId,
                UserFullName = $"{user.Name} {user.Surname}",
                UserProfilePhotoId = user.PhotographId
            };

        var resumeResult = await resumeQuery.SingleAsync(cancellationToken);

        resumeResult.Skills = await GetListResumeSkillByUserAsync(resumeResult.UserId, cancellationToken);
        resumeResult.EducationSteps = await GetListEducationStepByUserAsync(resumeResult.UserId, cancellationToken);
        resumeResult.ExperienceSteps = await GetListExperienceStepByUserAsync(resumeResult.UserId, cancellationToken);

        return resumeResult;
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