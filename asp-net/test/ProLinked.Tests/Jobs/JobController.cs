using ProLinked.API.Controllers.Jobs;
using ProLinked.TestBase;

namespace ProLinked.Tests.Jobs;

public class JobControllerTests: ControllerTestBase<JobController>
{
    public JobControllerTests(
        ProLinkedTestData testData,
        JobController controller)
        : base(testData, controller)
    {
    }
}