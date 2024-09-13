using ProLinked.Domain;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Application.Services.Resumes;

public interface IExperienceRepository: IRepository<ExperienceStep, Guid>
{
}
