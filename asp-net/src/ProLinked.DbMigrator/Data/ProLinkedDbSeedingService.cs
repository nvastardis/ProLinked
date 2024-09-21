using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.DbMigrator.Data;

public class ProLinkedDbSeedingService
{
    private readonly Dictionary<string, string> _pathToDataFile = new()
    {
        { "Chats", "Data\\Chats\\ChatData.json" },
        { "ChatMemberships", "Data\\Chats\\ChatMembershipData.json" },
        { "Messages", "Data\\Chats\\MessageData.json" },
        { "Connections", "Data\\Connections\\ConnectionData.json" },
        { "ConnectionRequests", "Data\\Connections\\ConnectionRequestData.json" },
        { "Advertisements", "Data\\Jobs\\AdvertisementData.json" },
        { "Applications", "Data\\Jobs\\ApplicationData.json" },
        { "Roles", "Data\\Identity\\RoleData.json" },
        { "Users", "Data\\Identity\\UserData.json" },
        { "Posts", "Data\\Posts\\PostData.json" },
        { "Reactions", "Data\\Posts\\ReactionData.json" },
        { "Comments", "Data\\Posts\\CommentData.json" },
        { "Skills", "Data\\Resumes\\SkillData.json" },
        { "Resumes", "Data\\Resumes\\ResumeData.json" },
        { "ResumeSkills", "Data\\Resumes\\ResumeSkillData.json" },
        { "ExperienceSteps", "Data\\Resumes\\ExperienceStepData.json" },
        { "ExperienceStepSkills", "Data\\Resumes\\ExperienceStepSkillData.json" },
        { "EducationSteps", "Data\\Resumes\\EducationStepData.json" },
        { "EducationStepSkills", "Data\\Resumes\\EducationStepSkillData.json" }
    };

    private ProLinkedDataSeeder DataSeeder{ get; }
    private ILogger Logger{ get; }


    public ProLinkedDbSeedingService(
        ILogger<ProLinkedDbSeedingService> logger,
        ProLinkedDataSeeder dataSeeder)
    {
        Logger = logger;
        DataSeeder = dataSeeder;
    }

    public async Task SeedAsync()
    {
        Logger.LogInformation("Starting database seeding...");
        await DataSeeder.SeedRolesIfNewAsync(
            nameof(IdentityRole<Guid>),
            _pathToDataFile["Roles"]);

        await DataSeeder.SeedUsersIfNewAsync(
            nameof(AppUser),
            _pathToDataFile["Users"]);

        await DataSeeder.SeedEntitiesIfNewAsync<Skill>(
            nameof(Skill),
            _pathToDataFile["Skills"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Resume>(
            nameof(Resume),
            _pathToDataFile["Resumes"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<ResumeSkill>(
            nameof(ResumeSkill),
            _pathToDataFile["ResumeSkills"],
            e=> x =>  e.ResumeId == x.ResumeId && e.SkillId == x.SkillId);

        await DataSeeder.SeedEntitiesIfNewAsync<EducationStep>(
            nameof(EducationStep),
            _pathToDataFile["EducationSteps"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<EducationStepSkill>(
            nameof(EducationStepSkill),
            _pathToDataFile["EducationStepSkills"],
            e=> x =>  e.EducationStepId == x.EducationStepId && e.SkillId == x.SkillId);

        await DataSeeder.SeedEntitiesIfNewAsync<ExperienceStep>(
            nameof(ExperienceStep),
            _pathToDataFile["ExperienceSteps"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<ExperienceStepSkill>(
            nameof(ExperienceStepSkill),
            _pathToDataFile["ExperienceStepSkills"],
            e=> x =>  e.ExperienceStepId == x.ExperienceStepId && e.SkillId == x.SkillId);

        await DataSeeder.SeedEntitiesIfNewAsync<Post>(
            nameof(Post),
            _pathToDataFile["Posts"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Comment>(
            nameof(Comment),
            _pathToDataFile["Comments"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Reaction>(
            nameof(Reaction),
            _pathToDataFile["Reactions"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Advertisement>(
            nameof(Advertisement),
            _pathToDataFile["Advertisements"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Application>(
            nameof(Application),
            _pathToDataFile["Applications"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<ConnectionRequest>(
            nameof(ConnectionRequest),
            _pathToDataFile["ConnectionRequests"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Connection>(
            nameof(Connection),
            _pathToDataFile["Connections"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<Chat>(
            nameof(Chat),
            _pathToDataFile["Chats"],
            e=> x =>  e.Id == x.Id);

        await DataSeeder.SeedEntitiesIfNewAsync<ChatMembership>(
            nameof(ChatMembership),
            _pathToDataFile["ChatMemberships"],
            e=> x =>  e.ChatId == x.ChatId && e.UserId == x.UserId);

        await DataSeeder.SeedEntitiesIfNewAsync<Message>(
            nameof(Message),
            _pathToDataFile["Messages"],
            e=> x =>  e.Id == x.Id);

        Logger.LogInformation("Successfully completed host database seeding.");
    }
}