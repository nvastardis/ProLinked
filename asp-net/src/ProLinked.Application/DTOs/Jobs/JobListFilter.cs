using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain.Shared.Jobs;

namespace ProLinked.Application.DTOs.Jobs;

public record JobListFilter: UserFilterDto
{
    public AdvertisementStatus AdvertisementStatus;
    public ApplicationStatus ApplicationStatus;
}