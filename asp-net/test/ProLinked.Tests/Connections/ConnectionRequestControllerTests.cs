using ProLinked.API.Controllers.Connections;
using ProLinked.TestBase;

namespace ProLinked.Tests.Connections;

public class ConnectionRequestControllerTests: ControllerTestBase<ConnectionRequestController>
{
    public ConnectionRequestControllerTests(
        ProLinkedTestData testData,
        ConnectionRequestController controller)
        : base(testData, controller)
    {
    }
}