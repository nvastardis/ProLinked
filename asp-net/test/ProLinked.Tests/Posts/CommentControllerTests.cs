using ProLinked.API.Controllers.Posts;
using ProLinked.TestBase;

namespace ProLinked.Tests.Posts;

public class CommentControllerTests: ControllerTestBase<CommentController>
{
    public CommentControllerTests(
        ProLinkedTestData testData,
        CommentController controller)
        : base(testData, controller)
    {
    }
}