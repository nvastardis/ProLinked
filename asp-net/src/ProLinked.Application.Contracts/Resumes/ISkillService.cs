using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Resumes;

public interface ISkillService
{
    Task<PagedAndSortedResultList<SkillDto>> GetSkillListAsync(
        CancellationToken cancellationToken = default);

    Task CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default);
}