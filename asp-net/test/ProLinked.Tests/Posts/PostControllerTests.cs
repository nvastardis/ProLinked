using ProLinked.API.Controllers.Posts;
using ProLinked.TestBase;

namespace ProLinked.Tests.Posts;

public class PostControllerTests: ControllerTestBase<PostController>
{
    public PostControllerTests(
        ProLinkedTestData testData,
        PostController controller)
        : base(testData, controller)
    {
    }
}