using ProLinked.Application.Contracts.Filtering;
using ProLinked.Domain.Shared.Jobs;

namespace ProLinked.Application.Contracts.Jobs.DTOs;

public record JobListFilter: UserFilterDto
{
    public AdvertisementStatus AdvertisementStatus;
    public ApplicationStatus ApplicationStatus;
}