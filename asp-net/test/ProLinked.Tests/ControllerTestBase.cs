using ProLinked.API.Controllers;

namespace ProLinked.TestBase;

public class ControllerTestBase<T>
    where T: ProLinkedController
{
    protected readonly ProLinkedTestData _testData;
    protected readonly T _controller;

    protected readonly DbContextFactory _dbContextFactory;

    public ControllerTestBase(
        ProLinkedTestData testData,
        T controller)
    {
        _testData = testData;
        _controller = controller;
        _dbContextFactory = new DbContextFactory();
    }
}