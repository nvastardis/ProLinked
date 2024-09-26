using ProLinked.Domain.Shared.Notifications;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.TestBase;

public class ProLinkedTestData
{
        /* USERS */
        public Guid UserAdminId { get; internal set; }
        public readonly string UserAdminUserName = "admin";
        public readonly string UserAdminName = "admin";

        public Guid UserJohnId = Guid.NewGuid();
        public readonly string UserJohnUserName = "john";
        public readonly string UserJohnName = "John";
        public readonly string UserJohnSurname = "A";

        public Guid UserGeorgeId = Guid.NewGuid();
        public readonly string UserGeorgeUserName = "george";
        public readonly string UserGeorgeName = "George";
        public readonly string UserGeorgeSurname = "B";

        /* CHATS */
        public Guid Chat1Id = Guid.NewGuid();
        public readonly string Chat1Title = "Chat1";
        public Guid Chat2Id = Guid.NewGuid();
        public readonly string Chat2Title = "Chat2";

        /* MESSAGES */
        public Guid Message1Id = Guid.NewGuid();
        public readonly string Message1Text = "Hi";
        public Guid Message2Id = Guid.NewGuid();
        public readonly string Message2Text = "Hi";

        /* POSTS */
        public Guid PostId = Guid.NewGuid();
        public readonly PostVisibilityEnum PostVisibility = PostVisibilityEnum.PUBLIC;
        public readonly string PostText = "Post Text";

        /* COMMENTS */
        public Guid CommentId = Guid.NewGuid();
        public readonly string CommentText = "Comment Text";

        /* REACTIONS */
        public Guid PostReactionId = Guid.NewGuid();
        public readonly ReactionTypeEnum PostReactionType = ReactionTypeEnum.FUNNY;
        public Guid CommentReactionId = Guid.NewGuid();
        public readonly ReactionTypeEnum CommentReactionType = ReactionTypeEnum.FUNNY;

        /* CONNECTIONS */
        public Guid ConnectionId = Guid.NewGuid();
        public Guid ConnectionRequest1Id = Guid.NewGuid();
        public Guid ConnectionRequest2Id = Guid.NewGuid();

        /* JOB */
        public Guid AdvertisementId = Guid.NewGuid();
        public Guid AdvertisementOpenId = Guid.NewGuid();
        public Guid AdvertisementClosedId = Guid.NewGuid();
        public readonly string AdTitle = "Title";
        public readonly string AdDescription = "Description";
        public readonly string AdCompany = "Company";
        public readonly string AdLocation = "Location";
        public readonly EmploymentTypeEnum EmploymentType = EmploymentTypeEnum.FULL;
        public readonly WorkArrangementEnum WorkArrangement = WorkArrangementEnum.REMOTE;
        public Guid ApplicationId = Guid.NewGuid();

        /* RESUME */
        public Guid EmptyResumeId = Guid.NewGuid();
        public Guid ResumeId = Guid.NewGuid();
        public Guid Skill1Id = Guid.NewGuid();
        public Guid Skill2Id = Guid.NewGuid();
        public Guid ExperienceStepId = Guid.NewGuid();
        public Guid EducationStepId = Guid.NewGuid();
        public DateTime StartDate = new(2000, 01, 01);
        public DateTime EndDate = new DateTime(2000, 01, 01);
        public readonly bool IsEmployed = false;
        public readonly string School = "School";
        public readonly string Degree = "Degree";
        public readonly string FieldOfStudy = "FieldOfStudy";
        public readonly string Grade = "Grade";
        public readonly string ActivitiesAndSocieties = "ActivitiesAndSocieties";
        public readonly string Skill1Title = "Skill1Title";
        public readonly string Skill2Title = "Skill2Title";

        /* NOTIFICATIONS */
        public Guid NotificationId = Guid.NewGuid();
        public NotificationTypeEnum NotificationType = NotificationTypeEnum.POST_REACTION;
}