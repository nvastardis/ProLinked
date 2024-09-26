using ProLinked.API.Controllers.Connections;
using ProLinked.TestBase;

namespace ProLinked.Tests.Connections;

public class ConnectionControllerTests: ControllerTestBase<ConnectionController>
{
    public ConnectionControllerTests(
        ProLinkedTestData testData,
        ConnectionController controller)
        : base(testData, controller)
    {
    }
}