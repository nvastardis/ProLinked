﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Domain.Chats;
using ProLinked.Domain.Connections;
using ProLinked.Domain.Identity;
using ProLinked.Domain.Jobs;
using ProLinked.Domain.Notifications;
using ProLinked.Domain.Posts;
using ProLinked.Domain.Resumes;
using ProLinked.Domain.Resumes.Education;
using ProLinked.Domain.Resumes.Experience;
using ProLinked.Domain.Resumes.Skills;

namespace ProLinked.Data;

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
