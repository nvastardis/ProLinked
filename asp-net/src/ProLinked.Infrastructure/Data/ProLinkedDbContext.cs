using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Entities.Notifications;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Infrastructure.Data;

public class ProLinkedDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMembership> ChatMemberships { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<ResumeSkill> ResumeSkills { get; set; }
    public DbSet<EducationStep> EducationSteps { get; set; }
    public DbSet<EducationStepSkill> EducationStepSkills { get; set; }
    public DbSet<ExperienceStep> ExperienceSteps { get; set; }
    public DbSet<ExperienceStepSkill> ExperienceStepSkills { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<ConnectionRequest> ConnectionRequests { get; set; }
    public DbSet<Blob> Blobs { get; set; }
    public DbSet<Advertisement> JobAdvertisements { get; set; }
    public DbSet<Application> JobApplications { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public ProLinkedDbContext(
        DbContextOptions<ProLinkedDbContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureBlobs();
        builder.ConfigureNotifications();
        builder.ConfigurePostEntities();
        builder.ConfigureChatEntities();
        builder.ConfigureResumeEntities();
        builder.ConfigureConnectionEntities();
        builder.ConfigureJobEntities();
    }
}