using ProLinked.Domain;

namespace ProLinked.Application.DTOs.Filtering;

public class ListFilterDto
{
    public bool IncludeDetails = false;
    public string? Sorting = null;
    public int SkipCount = ProLinkedConsts.SkipCountDefaultValue;
    public int MaxResultCount = ProLinkedConsts.MaxResultCountDefaultValue;
}