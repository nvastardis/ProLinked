using ProLinked.Domain.Shared.Jobs;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Application.Contracts.Jobs.DTOs;

public class AdvertisementCUDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string Location { get; init; } = null!;
    public EmploymentTypeEnum EmploymentType { get; init; }
    public WorkArrangementEnum WorkArrangement { get; init; }
    public AdvertisementStatus Status { get; init; }
}