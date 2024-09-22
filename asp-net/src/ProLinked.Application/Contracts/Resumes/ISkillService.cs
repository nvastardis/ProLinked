using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Resumes;

namespace ProLinked.Application.Contracts.Resumes;

public interface ISkillService
{
    Task<PagedAndSortedResultList<SkillDto>> GetSkillListAsync(
        CancellationToken cancellationToken = default);

    Task CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default);
}