using ProLinked.Domain;

namespace ProLinked.Application.DTOs.Filtering;

public record ListFilterDto
{
    public bool IncludeDetails = false;
    public string? Sorting;
    public int SkipCount;
    public int MaxResultCount;
    public DateTime? From;
    public DateTime? To;
}