using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Application.DTOs.Resumes;

public class ExperienceStepDto
{
    public Guid Id;
    public string Title = null!;
    public string Company = null!;
    public EmploymentTypeEnum EmploymentType;
    public bool IsEmployed;
    public string Location = null!;
    public WorkArrangementEnum WorkArrangement;
    public string? Description;
    public DateTime? StartDate;
    public DateTime? EndDate;
    public string[]? Skills;
}