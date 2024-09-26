using ProLinked.API.Controllers.Chats;
using ProLinked.TestBase;

namespace ProLinked.Tests.Chats;

public class ChatControllerTests : ControllerTestBase<ChatController>
{
    public ChatControllerTests(
        ProLinkedTestData testData,
        ChatController controller)
         : base(testData, controller)
    {
    }
}