using ProLinked.API.Controllers.Resumes;
using ProLinked.TestBase;

namespace ProLinked.Tests.Resumes;

public class SkillControllerTests: ControllerTestBase<SkillController>
{
    public SkillControllerTests(
        ProLinkedTestData testData,
        SkillController controller)
        : base(testData, controller)
    {
    }
}