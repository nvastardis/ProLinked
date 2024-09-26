using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Entities.Notifications;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Domain.Shared.Connections;
using ProLinked.Domain.Shared.Jobs;

namespace ProLinked.TestBase;

public class DataSeeder
{
    private readonly ProLinkedTestData _testData;
    private readonly UserManager<AppUser> _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IPostRepository _postRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly IConnectionRequestRepository _connectionRequestRepository;
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly IResumeRepository _resumeRepository;
    private readonly INotificationRepository _notificationRepository;

    public DataSeeder(
        ProLinkedTestData testData,
        UserManager<AppUser> userRepository,
        IChatRepository chatRepository,
        IPostRepository postRepository,
        IConnectionRepository connectionRepository,
        IConnectionRequestRepository connectionRequestRepository,
        IAdvertisementRepository advertisementRepository,
        ISkillRepository skillRepository,
        IResumeRepository resumeRepository,
        INotificationRepository notificationRepository)
    {
        _testData = testData;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _postRepository = postRepository;
        _connectionRequestRepository = connectionRequestRepository;
        _connectionRepository = connectionRepository;
        _advertisementRepository = advertisementRepository;
        _skillRepository = skillRepository;
        _resumeRepository = resumeRepository;
        _notificationRepository = notificationRepository;
    }
    public async Task SeedAsync()
    {
        await CreateUsersAsync();
        await CreateChatAsync();
        await CreatePostAsync();
        await CreateConnectionsAsync();
        await CreateJobAsync();
        await CreateResumeAsync();
        await CreateNotificationsAsync();
    }

    private async Task CreateUsersAsync()
    {
        var adminUser = await _userRepository.FindByNameAsync("ADMIN");
        _testData.UserAdminId = adminUser!.Id;

        var newAppUser = new AppUser
        {
            Id = _testData.UserJohnId,
            UserName =_testData.UserJohnName,
            Email = "john@mail.com",
            Name = _testData.UserJohnName,
            Surname = _testData.UserJohnSurname,
        };
        await _userRepository.CreateAsync(newAppUser);


        newAppUser = new AppUser
        {
            Id = _testData.UserGeorgeId,
            UserName =_testData.UserGeorgeName,
            Email = "george@mail.com",
            Name = _testData.UserGeorgeName,
            Surname = _testData.UserGeorgeSurname,
        };
        await _userRepository.CreateAsync(newAppUser);
    }

    private async Task CreateChatAsync()
    {
        var newChat = new Chat(_testData.Chat1Id);
        var newMessage = new Message(
            _testData.Message1Id,
            _testData.Chat1Id,
            _testData.UserJohnId,
            null,
            _testData.Message1Text);
        newChat.SetTitle(_testData.Chat1Title);
        newChat.AddMember(new ChatMembership(_testData.Chat1Id, _testData.UserJohnId));
        newChat.AddMember(new ChatMembership(_testData.Chat1Id, _testData.UserGeorgeId));
        newChat.AddMessage(newMessage);
        await _chatRepository.InsertAsync(newChat, autoSave:true);

        newChat = new Chat(_testData.Chat2Id);
        var newMessage2 = new Message(
            _testData.Message2Id,
            _testData.Chat2Id,
            _testData.UserJohnId,
            null,
            _testData.Message2Text);
        typeof(Message).GetProperty(nameof(Message.CreationTime))!.SetValue(newMessage2, DateTime.Now.AddMinutes(1));
        newChat.SetTitle(_testData.Chat2Title);
        newChat.AddMember(new ChatMembership(_testData.Chat2Id, _testData.UserJohnId));
        newChat.AddMember(new ChatMembership(_testData.Chat2Id, _testData.UserGeorgeId));
        newChat.AddMember(new ChatMembership(_testData.Chat2Id, _testData.UserAdminId));
        newChat.AddMessage(newMessage2);
        await _chatRepository.InsertAsync(newChat, autoSave:true);
    }

    private async Task CreatePostAsync()
    {
        var newPost = new Post(
            _testData.PostId,
            _testData.UserJohnId,
            _testData.PostVisibility);
        newPost.SetContent(_testData.PostText);

        var newPostReaction = new Reaction(
            _testData.PostId,
            _testData.UserGeorgeId,
            _testData.PostReactionType,
            postId: _testData.PostReactionId);


        newPost.AddReaction(newPostReaction);

        var newComment = new Comment(
            _testData.CommentId,
            _testData.PostId,
            _testData.UserGeorgeId);
        newComment.SetContent(_testData.CommentText);

        var newCommentReaction = new Reaction(
            _testData.CommentReactionId,
            _testData.UserGeorgeId,
            _testData.CommentReactionType,
            commentId: _testData.CommentId);

        newComment.AddReaction(newCommentReaction);
        newPost.AddComment(newComment);

        await _postRepository.InsertAsync(newPost, autoSave:true);
    }

    private async Task CreateConnectionsAsync()
    {
        var newConnectionRequest = new ConnectionRequest(
            _testData.ConnectionRequest1Id,
            _testData.UserAdminId,
            _testData.UserJohnId);
        await _connectionRequestRepository.InsertAsync(newConnectionRequest, autoSave: true);

        newConnectionRequest = new ConnectionRequest(
            _testData.ConnectionRequest2Id,
            _testData.UserAdminId,
            _testData.UserGeorgeId);
        newConnectionRequest.SetStatus(ConnectionRequestStatus.ACCEPTED);
        await _connectionRequestRepository.InsertAsync(newConnectionRequest, autoSave: true);

        var newConnection = new Connection(
            _testData.ConnectionId,
            _testData.UserAdminId,
            _testData.UserGeorgeId);
        await _connectionRepository.InsertAsync(newConnection, autoSave: true);
    }

    private async Task CreateJobAsync()
    {
        var newAdvertisement = new Advertisement(
            _testData.AdvertisementId,
            _testData.UserAdminId,
            _testData.AdTitle,
            _testData.AdDescription,
            _testData.AdCompany,
            _testData.AdLocation,
            _testData.EmploymentType,
            _testData.WorkArrangement);
        var newApplication = new Application(
            _testData.ApplicationId,
            _testData.AdvertisementId,
            _testData.UserJohnId);
        newAdvertisement.AddApplication(newApplication);
        await _advertisementRepository.InsertAsync(newAdvertisement, autoSave: true);

        newAdvertisement = new Advertisement(
            _testData.AdvertisementClosedId,
            _testData.UserAdminId,
            _testData.AdTitle,
            _testData.AdDescription,
            _testData.AdCompany,
            _testData.AdLocation,
            _testData.EmploymentType,
            _testData.WorkArrangement);
        newAdvertisement.SetStatus(AdvertisementStatus.CLOSED);
        await _advertisementRepository.InsertAsync(newAdvertisement, autoSave: true);

        newAdvertisement = new Advertisement(
            _testData.AdvertisementOpenId,
            _testData.UserAdminId,
            _testData.AdTitle,
            _testData.AdDescription,
            _testData.AdCompany,
            _testData.AdLocation,
            _testData.EmploymentType,
            _testData.WorkArrangement);
        await _advertisementRepository.InsertAsync(newAdvertisement, autoSave: true);
    }

    private async Task CreateResumeAsync()
    {

        var newSkill = new Skill(
            _testData.Skill1Id,
            _testData.Skill1Title);
        await _skillRepository.InsertAsync(newSkill, autoSave:true);

        newSkill = new Skill(
            _testData.Skill2Id,
            _testData.Skill2Title);
        await _skillRepository.InsertAsync(newSkill, autoSave:true);

        var newResume = new Resume(
            _testData.ResumeId,
            _testData.UserAdminId);

        var newResumeSkill = new ResumeSkill(
            _testData.ResumeId,
            _testData.Skill1Id,
            true);
        newResume.AddResumeSkill(newResumeSkill);

        var newEducation = new EducationStep(
            _testData.EducationStepId,
            _testData.ResumeId,
            _testData.School,
            _testData.Degree,
            _testData.FieldOfStudy,
            _testData.Grade,
            _testData.ActivitiesAndSocieties,
            _testData.AdDescription,
            _testData.StartDate,
            _testData.EndDate
        );
        var newEducationSkill = new EducationStepSkill(
            _testData.EducationStepId,
            _testData.Skill1Id);
        newEducation.AddRelatedSkill(newEducationSkill);

        var newExperience = new ExperienceStep(
            _testData.ExperienceStepId,
            _testData.ResumeId,
            _testData.AdTitle,
            _testData.AdCompany,
            _testData.EmploymentType,
            _testData.IsEmployed,
            _testData.AdLocation,
            _testData.WorkArrangement,
            _testData.AdDescription,
            _testData.StartDate,
            _testData.EndDate
        );
        var newExperienceSkill = new ExperienceStepSkill(
            _testData.ExperienceStepId,
            _testData.Skill1Id);
        newExperience.AddRelatedSkill(newExperienceSkill);
        newResume.AddExperienceStep(newExperience);
        newResume.AddEducationStep(newEducation);
        await _resumeRepository.InsertAsync(newResume, autoSave:true);

        newResume = new Resume(
            _testData.EmptyResumeId,
            _testData.UserJohnId);
        await _resumeRepository.InsertAsync(newResume, autoSave: true);
    }

    private async Task CreateNotificationsAsync()
    {
        var newNotification = new Notification(
            _testData.NotificationId,
            _testData.UserAdminId,
            _testData.UserJohnId,
            _testData.PostReactionId,
            _testData.NotificationType);
        await _notificationRepository.InsertAsync(newNotification, autoSave: true);
    }
}