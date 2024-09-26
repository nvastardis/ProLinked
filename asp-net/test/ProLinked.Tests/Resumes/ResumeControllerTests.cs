using ProLinked.API.Controllers.Resumes;
using ProLinked.TestBase;

namespace ProLinked.Tests.Resumes;

public class ResumeControllerTests: ControllerTestBase<ResumeController>
{
    public ResumeControllerTests(
        ProLinkedTestData testData,
        ResumeController controller)
        : base(testData, controller)
    {
    }
}